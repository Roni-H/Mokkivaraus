using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;
using MokkiVaraus_MAUI.Services;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class ReportsViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private readonly ReportService _reportService;
    private bool _isBusy;
    private DateTime _fromDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateTime _toDate = DateTime.Today.AddDays(30);
    private Area? _selectedArea;

    public ReportsViewModel(AppDatabase database, ReportService reportService)
    {
        _database = database;
        _reportService = reportService;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public ObservableCollection<Area> Areas { get; } = new();
    public ObservableCollection<BookingReportRow> AccommodationRows { get; } = new();
    public ObservableCollection<ServiceReportRow> ServiceRows { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public DateTime FromDate { get => _fromDate; set => SetProperty(ref _fromDate, value); }
    public DateTime ToDate { get => _toDate; set => SetProperty(ref _toDate, value); }

    public Area? SelectedArea
    {
        get => _selectedArea;
        set => SetProperty(ref _selectedArea, value);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Areas.Clear();
            foreach (var area in await _database.GetAreasAsync())
                Areas.Add(area);

            SelectedArea ??= Areas.FirstOrDefault();

            var areaId = SelectedArea?.Id;

            AccommodationRows.Clear();
            foreach (var row in await _reportService.GetAccommodationReportAsync(FromDate, ToDate, areaId))
                AccommodationRows.Add(row);

            ServiceRows.Clear();
            foreach (var row in await _reportService.GetServiceReportAsync(FromDate, ToDate, areaId))
                ServiceRows.Add(row);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
