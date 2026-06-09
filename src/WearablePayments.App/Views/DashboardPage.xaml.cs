namespace WearablePayments.App.Views;
using WearablePayments.App.ViewModels;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing() => ((DashboardViewModel)BindingContext).LoadCommand.Execute(null);
}
