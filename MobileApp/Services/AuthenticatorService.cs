using System.Text.Json;
using OtpNet;
#if ANDROID || IOS || MACCATALYST
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
#endif
namespace JPYCOffline.Services;

public class AuthenticatorService : IAuthenticatorService
{
    public async Task<bool> AuthenticateAsync()
    {
#if ANDROID || IOS || MACCATALYST
        var request = new Plugin.Fingerprint.Abstractions.AuthenticationRequestConfiguration(
            "Biometric Authentication", "Authenticate to continue");
        var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(request);
        return result.Authenticated;
#else
        // Stub for unsupported platforms
        await Task.CompletedTask;
        return true;
#endif
    }

    const string AccountsKey = "auth_accounts";
    readonly IKeyValueStore _store;
    readonly object _lock = new();
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
        lock (_lock)
        {
            if (_loaded) return;
            if (!string.IsNullOrEmpty(json))
                _accounts = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            _loaded = true;
        }
    }

    async Task PersistAsync()
    {
        Dictionary<string, string> snapshot;
        lock (_lock)
        {
            snapshot = new Dictionary<string, string>(_accounts);
        }
        await _store.SetAsync(AccountsKey, JsonSerializer.Serialize(snapshot));
    }

    string Key(string issuer, string account) => $"{issuer}:{account}";


    public async Task AddAccountAsync(string issuer, string accountName, string secret)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            _accounts[Key(issuer, accountName)] = secret;
        }
        await PersistAsync();
    }

    public async Task RemoveAccountAsync(string issuer, string accountName)
    {
        await EnsureLoadedAsync();
        lock (_lock)
        {
            _accounts.Remove(Key(issuer, accountName));
        }
        await PersistAsync();
    }

    public async Task<IReadOnlyList<AuthenticatorAccount>> GetAccountsAsync()
    {
        await EnsureLoadedAsync();
        List<AuthenticatorAccount> result;
        lock (_lock)
        {
            result = _accounts.Keys.Select(k =>
            {
                var parts = k.Split(':', 2);
                return new AuthenticatorAccount(parts[0], parts[1]);
            }).ToList();
        }
        return result;
    }

    public async Task<string> GenerateTotpAsync(string issuer, string accountName)
    {
        await EnsureLoadedAsync();
        string secret;
        lock (_lock)
        {
            if (!_accounts.TryGetValue(Key(issuer, accountName), out var s))
                throw new InvalidOperationException("Account not found");
            secret = s;
        }
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
