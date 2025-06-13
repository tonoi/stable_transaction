namespace OfflineGuide.Services.Travel;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> BiometricLoginAsync();
    Task<bool> PasscodeLoginAsync(string passcode);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string> GetCurrentUserAsync();
}

public class AuthenticationService : IAuthenticationService
{
    private bool _isAuthenticated = false;
    private string _currentUser = string.Empty;

    public async Task<bool> LoginAsync(string username, string password)
    {
        await Task.Delay(1000); // Simulate network call

        // Simple mock authentication
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            _isAuthenticated = true;
            _currentUser = username;
            return true;
        }

        return false;
    }

    public async Task<bool> BiometricLoginAsync()
    {
        await Task.Delay(500); // Simulate biometric check

        try
        {
            // In a real app, you would use Plugin.Fingerprint here
            _isAuthenticated = true;
            _currentUser = "BiometricUser";
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> PasscodeLoginAsync(string passcode)
    {
        await Task.Delay(300); // Simulate passcode check

        // Simple mock passcode validation
        if (!string.IsNullOrEmpty(passcode) && passcode.Length >= 4)
        {
            _isAuthenticated = true;
            _currentUser = "PasscodeUser";
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await Task.Delay(100);
        _isAuthenticated = false;
        _currentUser = string.Empty;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        await Task.Delay(50);
        return _isAuthenticated;
    }

    public async Task<string> GetCurrentUserAsync()
    {
        await Task.Delay(50);
        return _currentUser;
    }
}
