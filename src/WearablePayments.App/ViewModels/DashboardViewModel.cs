namespace WearablePayments.App.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WearablePayments.App.Services;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<TransactionItem> _transactions = new();
    [ObservableProperty] private bool _isBusy;

    public DashboardViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var txs = await _api.GetAsync<List<TransactionItem>>("api/transactions");
            Transactions.Clear();
            foreach (var tx in txs ?? [])
                Transactions.Add(tx);
        }
        finally { IsBusy = false; }
    }
}

public record TransactionItem(
    Guid Id, decimal Amount, string Currency, string MerchantName,
    string Status, string AuthorizationCode, DateTime CreatedAt, string DeviceName);
