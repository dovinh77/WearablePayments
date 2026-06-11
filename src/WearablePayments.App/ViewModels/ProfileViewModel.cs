namespace WearablePayments.App.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WearablePayments.App.Services;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _initials = string.Empty;
    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmNewPassword = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isSuccess;
    [ObservableProperty] private bool _isBusy;

    public ProfileViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            var profile = await _api.GetAsync<UserProfile>("api/profile");
            if (profile is not null)
            {
                FullName = profile.FullName;
                Email = profile.Email;
                Initials = BuildInitials(profile.FullName);
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task UpdateNameAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName)) return;
        StatusMessage = string.Empty;
        IsBusy = true;
        try
        {
            await _api.PostAsync<object>("api/profile/update-name", new { fullName = FullName });
            Initials = BuildInitials(FullName);
            IsSuccess = true;
            StatusMessage = "Name updated successfully.";
        }
        catch
        {
            IsSuccess = false;
            StatusMessage = "Failed to update name.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            IsSuccess = false;
            StatusMessage = "All password fields are required.";
            return;
        }

        if (NewPassword != ConfirmNewPassword)
        {
            IsSuccess = false;
            StatusMessage = "New passwords do not match.";
            return;
        }

        if (NewPassword.Length < 8)
        {
            IsSuccess = false;
            StatusMessage = "New password must be at least 8 characters.";
            return;
        }

        IsBusy = true;
        try
        {
            await _api.PostAsync<object>("api/profile/change-password",
                new { currentPassword = CurrentPassword, newPassword = NewPassword });
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            IsSuccess = true;
            StatusMessage = "Password changed successfully.";
        }
        catch
        {
            IsSuccess = false;
            StatusMessage = "Failed to change password. Check your current password.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        _api.SetToken(string.Empty);
        await Shell.Current.GoToAsync("//Login");
    }

    private static string BuildInitials(string name) =>
        string.Concat(name.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(w => char.ToUpper(w[0])));
}

public record UserProfile(string FullName, string Email);
