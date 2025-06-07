using System.ComponentModel;
using JPYCOffline.Services;

namespace JPYCOffline.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    readonly UserService _userSvc;
    public event PropertyChangedEventHandler? PropertyChanged;

    string _username = "";
    public string Username { get => _username; set { _username = value; PropertyChanged?.Invoke(this, new(nameof(Username))); } }
    string _password = "";
    public string Password { get => _password; set { _password = value; PropertyChanged?.Invoke(this, new(nameof(Password))); } }

    public LoginViewModel(UserService svc) => _userSvc = svc;

    public async Task OnSignInAsync()
    {
        if (await _userSvc.SignInAsync(Username, Password))
            await Shell.Current.GoToAsync("/wallet");
        else
            await Shell.Current.DisplayAlert("Error", "Sign in failed", "OK");
    }
}
