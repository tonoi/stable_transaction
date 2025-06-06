using System.Text.Json;
using OtpNet;
namespace JPYCOffline.Services;

public class AuthenticatorService : IAuthenticatorService
{
    public Task<bool> AuthenticateAsync()
    {
        // TODO: integrate platform-specific biometric authentication
        return Task.FromResult(true);
    }

    const string AccountsKey = "auth_accounts";
    readonly IKeyValueStore _store;
    Dictionary<string, string> _accounts = new();
    bool _loaded = false;

    public AuthenticatorService(IKeyValueStore? store = null)
    {
#if ANDROID || IOS || MACCATALYST
        _store = store ?? new SecureStorageKeyValueStore();
#else
        _store = store ?? new MemoryKeyValueStore();
#endif
    }

    async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        var json = await _store.GetAsync(AccountsKey);
        if (!string.IsNullOrEmpty(json))
            _accounts = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        _loaded = true;
    }

    async Task PersistAsync() => await _store.SetAsync(AccountsKey, JsonSerializer.Serialize(_accounts));

    string Key(string issuer, string account) => $"{issuer}:{account}";

    public Task<bool> AuthenticateAsync() => Task.FromResult(true);

    public async Task AddAccountAsync(string issuer, string accountName, string secret)
    {
        await EnsureLoadedAsync();
        _accounts[Key(issuer, accountName)] = secret;
        await PersistAsync();
    }

    public async Task RemoveAccountAsync(string issuer, string accountName)
    {
        await EnsureLoadedAsync();
        _accounts.Remove(Key(issuer, accountName));
        await PersistAsync();
    }

    public async Task<IReadOnlyList<AuthenticatorAccount>> GetAccountsAsync()
    {
        await EnsureLoadedAsync();
        return _accounts.Keys.Select(k =>
        {
            var parts = k.Split(':', 2);
            return new AuthenticatorAccount(parts[0], parts[1]);
        }).ToList();
    }

    public string GenerateTotp(string issuer, string accountName)
    {
        EnsureLoadedAsync().GetAwaiter().GetResult();
        if (!_accounts.TryGetValue(Key(issuer, accountName), out var secret))
            throw new InvalidOperationException("Account not found");
        var totp = new Totp(Base32Encoding.ToBytes(secret));
        return totp.ComputeTotp(DateTime.UtcNow);
    }
}

#if !(ANDROID || IOS || MACCATALYST)
internal class MemoryKeyValueStore : IKeyValueStore
{
    static readonly Dictionary<string, string> _mem = new();
    public Task SetAsync(string key, string value) { _mem[key] = value; return Task.CompletedTask; }
    public Task<string?> GetAsync(string key) => Task.FromResult(_mem.TryGetValue(key, out var v) ? v : null);
    public Task RemoveAsync(string key) { _mem.Remove(key); return Task.CompletedTask; }
}
#endif
