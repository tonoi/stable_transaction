namespace JPYCOffline.Services;

public record AuthenticatorAccount(string Issuer, string AccountName);

public interface IAuthenticatorService
{
    Task<bool> AuthenticateAsync();

    Task AddAccountAsync(string issuer, string accountName, string secret);

    Task RemoveAccountAsync(string issuer, string accountName);

    Task<IReadOnlyList<AuthenticatorAccount>> GetAccountsAsync();

    Task<string> GenerateTotpAsync(string issuer, string accountName);
}