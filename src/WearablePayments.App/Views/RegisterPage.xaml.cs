namespace WearablePayments.App.Views;
using WearablePayments.App.ViewModels;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
