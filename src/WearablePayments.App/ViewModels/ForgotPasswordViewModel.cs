namespace WearablePayments.App.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WearablePayments.App.Services;

public partial class ForgotPasswordViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isSuccess;

    public ForgotPasswordViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private async Task SendResetAsync()
    {
        StatusMessage = string.Empty;
        IsBusy = true;
        try
        {
            await _api.PostAsync<object>("api/auth/forgot-password", new { email = Email });
            IsSuccess = true;
            StatusMessage = "If that email exists, a reset link has been sent.";
        }
        catch
        {
            IsSuccess = true; // Don't reveal whether the email exists
            StatusMessage = "If that email exists, a reset link has been sent.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task BackToLoginAsync() =>
        await Shell.Current.GoToAsync("//Login");
}
