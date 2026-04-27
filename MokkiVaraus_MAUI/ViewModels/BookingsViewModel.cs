using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;
using MokkiVaraus_MAUI.Services;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class BookingsViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private readonly InvoiceService _invoiceService;
    private readonly ReportService _reportService;
    private bool _isBusy;
    private BookingReportRow? _selectedBookingRow;

    private Cottage? _selectedCottage;
    private Customer? _selectedCustomer;
    private ExtraService? _selectedService;
    private BookingStatus _selectedStatus = BookingStatus.Confirmed;
    private InvoiceDeliveryMethod _selectedDeliveryMethod = InvoiceDeliveryMethod.Email;
    private string _quantityText = "1";
    private string _notes = string.Empty;
    private DateTime _startDate = DateTime.Today.AddDays(7);
    private DateTime _endDate = DateTime.Today.AddDays(10);

    public BookingsViewModel(AppDatabase database, InvoiceService invoiceService, ReportService reportService)
    {
        _database = database;
        _invoiceService = invoiceService;
        _reportService = reportService;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedBookingRow is not null);
        AddServiceCommand = new AsyncRelayCommand(AddServiceAsync, () => SelectedBookingRow is not null && SelectedService is not null);
        CreateInvoiceCommand = new AsyncRelayCommand(CreateInvoiceAsync, () => SelectedBookingRow is not null);
        ClearCommand = new RelayCommand(ClearForm);
    }

    public ObservableCollection<BookingReportRow> Bookings { get; } = new();
    public ObservableCollection<Cottage> Cottages { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<ExtraService> Services { get; } = new();
    public ObservableCollection<BookingExtraService> SelectedBookingServices { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public AsyncRelayCommand AddServiceCommand { get; }
    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public RelayCommand ClearCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public BookingReportRow? SelectedBookingRow
    {
        get => _selectedBookingRow;
        set
        {
            if (SetProperty(ref _selectedBookingRow, value))
            {
                if (value != null){
                    _ = LoadSelectedBookingServicesAsync();
                }
                DeleteCommand.RaiseCanExecuteChanged();
                AddServiceCommand.RaiseCanExecuteChanged();
                CreateInvoiceCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Cottage? SelectedCottage { get => _selectedCottage; set => SetProperty(ref _selectedCottage, value); }
    public Customer? SelectedCustomer { get => _selectedCustomer; set => SetProperty(ref _selectedCustomer, value); }
    public ExtraService? SelectedService { get => _selectedService; set => SetProperty(ref _selectedService, value); }

    public BookingStatus SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }
    public InvoiceDeliveryMethod SelectedDeliveryMethod { get => _selectedDeliveryMethod; set => SetProperty(ref _selectedDeliveryMethod, value); }

    public string QuantityText { get => _quantityText; set => SetProperty(ref _quantityText, value); }
    public string Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public DateTime StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }
    public DateTime EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Cottages.Clear();
            foreach (var cottage in await _database.GetCottagesAsync())
                Cottages.Add(cottage);

            Customers.Clear();
            foreach (var customer in await _database.GetCustomersAsync())
                Customers.Add(customer);

            Services.Clear();
            foreach (var service in await _database.GetExtraServicesAsync())
                Services.Add(service);

            Bookings.Clear();
            var report = await _reportService.GetAccommodationReportAsync(DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(6));
            foreach (var row in report)
                Bookings.Add(row);

            SelectedBookingRow ??= Bookings.FirstOrDefault();
            SelectedCottage ??= Cottages.FirstOrDefault();
            SelectedCustomer ??= Customers.FirstOrDefault();
            SelectedService ??= Services.FirstOrDefault();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearForm()
    {
        SelectedBookingRow = null;
        SelectedCottage = Cottages.FirstOrDefault();
        SelectedCustomer = Customers.FirstOrDefault();
        SelectedService = Services.FirstOrDefault();
        SelectedStatus = BookingStatus.Confirmed;
        SelectedDeliveryMethod = InvoiceDeliveryMethod.Email;
        QuantityText = "1";
        Notes = string.Empty;
        StartDate = DateTime.Today.AddDays(7);
        EndDate = DateTime.Today.AddDays(10);
        SelectedBookingServices.Clear();
    }

    private async Task SaveAsync()
    {
        if (SelectedCottage is null || SelectedCustomer is null)
            return;

        if (StartDate >= EndDate)
            return;

        var booking = new Booking
        {
            CottageId = SelectedCottage.Id,
            CustomerId = SelectedCustomer.Id,
            StartDate = StartDate,
            EndDate = EndDate,
            Status = SelectedStatus,
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
        };

        await _database.SaveBookingAsync(booking);

        if (SelectedService is not null && int.TryParse(QuantityText, out var qty) && qty > 0)
        {
            await _database.SaveBookingExtraServiceAsync(new BookingExtraService
            {
                BookingId = booking.Id,
                ExtraServiceId = SelectedService.Id,
                Quantity = qty,
                UnitPrice = SelectedService.Price
            });
        }

        await LoadAsync();
        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedBookingRow is null)
            return;

        var booking = await _database.GetBookingByIdAsync(SelectedBookingRow.BookingId);
        if (booking is null)
            return;

        await _database.DeleteBookingAsync(booking);
        await LoadAsync();
        ClearForm();
    }

    private async Task AddServiceAsync()
    {
        if (SelectedBookingRow is null || SelectedService is null)
            return;

        if (!int.TryParse(QuantityText, out var qty) || qty <= 0)
            return;

        await _database.SaveBookingExtraServiceAsync(new BookingExtraService
        {
            BookingId = SelectedBookingRow.BookingId,
            ExtraServiceId = SelectedService.Id,
            Quantity = qty,
            UnitPrice = SelectedService.Price
        });

        await LoadSelectedBookingServicesAsync();
    }

    private async Task CreateInvoiceAsync()
    {
        if (SelectedBookingRow is null)
            return;

        var booking = await _database.GetBookingByIdAsync(SelectedBookingRow.BookingId);
        var cottage = booking is null ? null : await _database.GetCottageByIdAsync(booking.CottageId);

        if (booking is null || cottage is null)
            return;

        var nights = Math.Max(1, (booking.EndDate - booking.StartDate).Days);
        var baseAmount = nights * cottage.NightlyPrice;

        var extras = await _database.GetBookingExtraServicesAsync();
        var extraAmount = extras
            .Where(x => x.BookingId == booking.Id)
            .Sum(x => x.UnitPrice * x.Quantity);

        await _invoiceService.CreateInvoiceForBookingAsync(
            booking,
            baseAmount + extraAmount,
            SelectedDeliveryMethod,
            "Luotu varauksen perusteella.");

        await LoadAsync();
    }

    private async Task LoadSelectedBookingServicesAsync()
    {
        SelectedBookingServices.Clear();

        var selected=SelectedBookingRow;
        if(selected==null)
            return;
        var services=await _database.GetBookingExtraServicesAsync();
        foreach (var row in services
                     .Where(x=>x.BookingId==selected.BookingId))
        {
            SelectedBookingServices.Add(row);
        }
    }
}
