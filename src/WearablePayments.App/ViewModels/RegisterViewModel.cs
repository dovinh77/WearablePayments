namespace WearablePayments.App.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WearablePayments.App.Services;

public partial class RegisterViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public RegisterViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        if (Password.Length < 8)
        {
            ErrorMessage = "Password must be at least 8 characters.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _api.PostAsync<AuthResult>("api/auth/register",
                new { email = Email, fullName = FullName, password = Password });

            if (result is not null)
            {
                _api.SetToken(result.Token);
                await Shell.Current.GoToAsync("//Dashboard");
            }
        }
        catch
        {
            ErrorMessage = "Registration failed. Email may already be in use.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task BackToLoginAsync() =>
        await Shell.Current.GoToAsync("//Login");
}
