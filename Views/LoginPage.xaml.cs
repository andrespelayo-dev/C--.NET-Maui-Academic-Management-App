using C971project.Services;

namespace C971project.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;

        if (!_authService.Login(username, password))
        {
            await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
            return;
        }

        Application.Current!.MainPage = new AppShell();
    }
}