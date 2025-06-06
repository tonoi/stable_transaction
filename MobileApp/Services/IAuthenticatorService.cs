namespace JPYCOffline.Services;

public interface IAuthenticatorService
{
    Task<bool> AuthenticateAsync();
    Task<bool> AuthenticateWithPasskeyAsync();
}