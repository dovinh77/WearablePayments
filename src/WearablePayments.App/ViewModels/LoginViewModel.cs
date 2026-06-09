namespace WearablePayments.App.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WearablePayments.App.Services;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public LoginViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            var result = await _api.PostAsync<AuthResult>("api/auth/login", new { email = Email, password = Password });
            if (result is not null)
            {
                _api.SetToken(result.Token);
                await Shell.Current.GoToAsync("//Dashboard");
            }
        }
        catch
        {
            ErrorMessage = "Invalid email or password.";
        }
        finally { IsBusy = false; }
    }
}

public record AuthResult(string Token, string Email, string FullName);
