namespace WearablePayments.App.Views;
using WearablePayments.App.ViewModels;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing() =>
        ((ProfileViewModel)BindingContext).LoadCommand.Execute(null);
}
