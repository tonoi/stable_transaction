namespace JPYCOffline.ViewModels;

public class LoginViewModel
{
    public async Task OnPasskeyAsync()
    {
        // TODO: implement real passkey authentication
        await Shell.Current.GoToAsync("/bioauth");
    }
}
