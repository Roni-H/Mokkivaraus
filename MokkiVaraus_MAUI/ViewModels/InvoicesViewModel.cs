using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;
using MokkiVaraus_MAUI.Services;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class InvoicesViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private readonly InvoiceService _invoiceService;
    private bool _isBusy;
    private Invoice? _selectedInvoice;

    public InvoicesViewModel(AppDatabase database, InvoiceService invoiceService)
    {
        _database = database;
        _invoiceService = invoiceService;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        MarkPaidCommand = new AsyncRelayCommand(MarkPaidAsync, () => SelectedInvoice is not null);
    }

    public ObservableCollection<Invoice> Invoices { get; } = new();
    public ObservableCollection<Invoice> OpenInvoices { get; } = new();
    public ObservableCollection<Invoice> OverdueInvoices { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand MarkPaidCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public Invoice? SelectedInvoice
    {
        get => _selectedInvoice;
        set
        {
            if (SetProperty(ref _selectedInvoice, value))
                MarkPaidCommand.RaiseCanExecuteChanged();
        }
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var invoices = await _database.GetInvoicesAsync();

            Invoices.Clear();
            foreach (var invoice in invoices)
                Invoices.Add(invoice);

            OpenInvoices.Clear();
            foreach (var invoice in await _invoiceService.GetOpenInvoicesAsync())
                OpenInvoices.Add(invoice);

            OverdueInvoices.Clear();
            foreach (var invoice in await _invoiceService.GetOverdueInvoicesAsync())
                OverdueInvoices.Add(invoice);

            SelectedInvoice ??= Invoices.FirstOrDefault();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task MarkPaidAsync()
    {
        if (SelectedInvoice is null)
            return;

        await _invoiceService.MarkAsPaidAsync(SelectedInvoice.Id);
        await LoadAsync();
    }
}
