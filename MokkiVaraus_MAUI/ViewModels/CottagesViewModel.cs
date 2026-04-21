using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class CottagesViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private bool _isBusy;
    private Cottage? _selectedCottage;
    private Area? _selectedArea;
    private Area? _filterArea;
    private string _filterPriceText = string.Empty;
    private string _filterEquipment = string.Empty;
    private DateTime _availableFrom = DateTime.Today;
    private DateTime _availableTo = DateTime.Today.AddDays(7);

    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _capacityText = "1";
    private string _priceText = "0";
    private string _equipment = string.Empty;
    private bool _isActive = true;

    public CottagesViewModel(AppDatabase database)
    {
        _database = database;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedCottage is not null);
        NewCommand = new RelayCommand(ClearForm);
    }

    public ObservableCollection<Cottage> Cottages { get; } = new();
    public ObservableCollection<Area> Areas { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand SearchCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public RelayCommand NewCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public Cottage? SelectedCottage
    {
        get => _selectedCottage;
        set
        {
            if (SetProperty(ref _selectedCottage, value))
            {
                LoadSelected();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Area? SelectedArea
    {
        get => _selectedArea;
        set => SetProperty(ref _selectedArea, value);
    }

    public Area? FilterArea
    {
        get => _filterArea;
        set => SetProperty(ref _filterArea, value);
    }

    public string FilterPriceText { get => _filterPriceText; set => SetProperty(ref _filterPriceText, value); }
    public string FilterEquipment { get => _filterEquipment; set => SetProperty(ref _filterEquipment, value); }
    public DateTime AvailableFrom { get => _availableFrom; set => SetProperty(ref _availableFrom, value); }
    public DateTime AvailableTo { get => _availableTo; set => SetProperty(ref _availableTo, value); }

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string CapacityText { get => _capacityText; set => SetProperty(ref _capacityText, value); }
    public string PriceText { get => _priceText; set => SetProperty(ref _priceText, value); }
    public string Equipment { get => _equipment; set => SetProperty(ref _equipment, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Areas.Clear();
            foreach (var area in await _database.GetAreasAsync())
                Areas.Add(area);

            FilterArea ??= Areas.FirstOrDefault();
            SelectedArea ??= Areas.FirstOrDefault();

            Cottages.Clear();
            foreach (var cottage in await _database.GetCottagesAsync())
                Cottages.Add(cottage);

            if (SelectedArea is not null)
                SelectedArea = Areas.FirstOrDefault(x => x.Id == SelectedArea.Id);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SearchAsync()
    {
        int? areaId = FilterArea?.Id;
        decimal? maxPrice = decimal.TryParse(FilterPriceText, out var price) ? price : null;

        Cottages.Clear();
        foreach (var cottage in await _database.SearchAvailableCottagesAsync(
                     areaId,
                     maxPrice,
                     FilterEquipment,
                     AvailableFrom,
                     AvailableTo))
        {
            Cottages.Add(cottage);
        }
    }

    private void LoadSelected()
    {
        if (SelectedCottage is null)
        {
            ClearForm();
            return;
        }

        SelectedArea = Areas.FirstOrDefault(x => x.Id == SelectedCottage.AreaId);
        Name = SelectedCottage.Name;
        Description = SelectedCottage.Description ?? string.Empty;
        CapacityText = SelectedCottage.Capacity.ToString();
        PriceText = SelectedCottage.NightlyPrice.ToString("0.##");
        Equipment = SelectedCottage.Equipment ?? string.Empty;
        IsActive = SelectedCottage.IsActive;
    }

    private void ClearForm()
    {
        SelectedCottage = null;
        SelectedArea = Areas.FirstOrDefault();
        Name = string.Empty;
        Description = string.Empty;
        CapacityText = "1";
        PriceText = "0";
        Equipment = string.Empty;
        IsActive = true;
    }

    private async Task SaveAsync()
    {
        if (SelectedArea is null || string.IsNullOrWhiteSpace(Name))
            return;

        if (!int.TryParse(CapacityText, out var capacity))
            return;

        if (!decimal.TryParse(PriceText, out var price))
            return;

        var cottage = SelectedCottage ?? new Cottage();
        cottage.AreaId = SelectedArea.Id;
        cottage.Name = Name.Trim();
        cottage.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        cottage.Capacity = capacity;
        cottage.NightlyPrice = price;
        cottage.Equipment = string.IsNullOrWhiteSpace(Equipment) ? null : Equipment.Trim();
        cottage.IsActive = IsActive;

        await _database.SaveCottageAsync(cottage);
        await LoadAsync();
        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCottage is null)
            return;

        await _database.DeleteCottageAsync(SelectedCottage);
        await LoadAsync();
        ClearForm();
    }
}
