namespace JPYCOffline.Services;

public class AuthenticatorService : IAuthenticatorService
{
    public Task<bool> AuthenticateAsync()
    {
        // TODO: integrate platform-specific biometric authentication
        return Task.FromResult(true);
    }
}
