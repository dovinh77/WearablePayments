namespace WearablePayments.App.Views;
using WearablePayments.App.ViewModels;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(ForgotPasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
