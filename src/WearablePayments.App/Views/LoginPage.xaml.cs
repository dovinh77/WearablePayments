namespace WearablePayments.App.Views;
using WearablePayments.App.ViewModels;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
