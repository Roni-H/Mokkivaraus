using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;
using MokkiVaraus_MAUI.Services;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private readonly ReportService _reportService;
    private readonly InvoiceService _invoiceService;

    private int _areaCount;
    private int _cottageCount;
    private int _bookingCount;
    private int _openInvoiceCount;
    private int _overdueInvoiceCount;
    private bool _isBusy;

    public DashboardViewModel(AppDatabase database, ReportService reportService, InvoiceService invoiceService)
    {
        _database = database;
        _reportService = reportService;
        _invoiceService = invoiceService;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public AsyncRelayCommand RefreshCommand { get; }

    public ObservableCollection<BookingReportRow> UpcomingBookings { get; } = new();
    public ObservableCollection<Invoice> OpenInvoices { get; } = new();
    public ObservableCollection<ServiceReportRow> TopServices { get; } = new();

    public int AreaCount { get => _areaCount; set => SetProperty(ref _areaCount, value); }
    public int CottageCount { get => _cottageCount; set => SetProperty(ref _cottageCount, value); }
    public int BookingCount { get => _bookingCount; set => SetProperty(ref _bookingCount, value); }
    public int OpenInvoiceCount { get => _openInvoiceCount; set => SetProperty(ref _openInvoiceCount, value); }
    public int OverdueInvoiceCount { get => _overdueInvoiceCount; set => SetProperty(ref _overdueInvoiceCount, value); }
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            await _database.InitializeAsync();

            var areas = await _database.GetAreasAsync();
            var cottages = await _database.GetCottagesAsync();
            var bookings = await _database.GetBookingsAsync();

            var openInvoices = await _invoiceService.GetOpenInvoicesAsync();
            var overdueInvoices = await _invoiceService.GetOverdueInvoicesAsync();

            AreaCount = areas.Count;
            CottageCount = cottages.Count;
            BookingCount = bookings.Count;
            OpenInvoiceCount = openInvoices.Count;
            OverdueInvoiceCount = overdueInvoices.Count;

            UpcomingBookings.Clear();
            var report = await _reportService.GetAccommodationReportAsync(DateTime.Today, DateTime.Today.AddDays(30));
            foreach (var row in report.Take(5))
                UpcomingBookings.Add(row);

            OpenInvoices.Clear();
            foreach (var invoice in openInvoices.Take(5))
                OpenInvoices.Add(invoice);

            TopServices.Clear();
            var serviceReport = await _reportService.GetServiceReportAsync(DateTime.Today.AddMonths(-1), DateTime.Today);
            foreach (var row in serviceReport
                         .GroupBy(x => x.ServiceName)
                         .Select(g => new ServiceReportRow
                         {
                             ServiceName = g.Key,
                             Quantity = g.Sum(x => x.Quantity),
                             Revenue = g.Sum(x => x.Revenue)
                         })
                         .OrderByDescending(x => x.Quantity)
                         .Take(5))
            {
                TopServices.Add(row);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
