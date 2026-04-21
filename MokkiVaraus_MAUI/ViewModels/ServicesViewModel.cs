using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class ServicesViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private bool _isBusy;
    private ExtraService? _selectedService;
    private Area? _selectedArea;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _priceText = "0";
    private bool _isActive = true;

    public ServicesViewModel(AppDatabase database)
    {
        _database = database;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedService is not null);
        NewCommand = new RelayCommand(ClearForm);
    }

    public ObservableCollection<ExtraService> Services { get; } = new();
    public ObservableCollection<Area> Areas { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public RelayCommand NewCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public ExtraService? SelectedService
    {
        get => _selectedService;
        set
        {
            if (SetProperty(ref _selectedService, value))
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

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string PriceText { get => _priceText; set => SetProperty(ref _priceText, value); }
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

            SelectedArea ??= Areas.FirstOrDefault();

            Services.Clear();
            foreach (var service in await _database.GetExtraServicesAsync())
                Services.Add(service);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadSelected()
    {
        if (SelectedService is null)
        {
            ClearForm();
            return;
        }

        SelectedArea = Areas.FirstOrDefault(x => x.Id == SelectedService.AreaId);
        Name = SelectedService.Name;
        Description = SelectedService.Description ?? string.Empty;
        PriceText = SelectedService.Price.ToString("0.##");
        IsActive = SelectedService.IsActive;
    }

    private void ClearForm()
    {
        SelectedService = null;
        SelectedArea = Areas.FirstOrDefault();
        Name = string.Empty;
        Description = string.Empty;
        PriceText = "0";
        IsActive = true;
    }

    private async Task SaveAsync()
    {
        if (SelectedArea is null || string.IsNullOrWhiteSpace(Name))
            return;

        if (!decimal.TryParse(PriceText, out var price))
            return;

        var service = SelectedService ?? new ExtraService();
        service.AreaId = SelectedArea.Id;
        service.Name = Name.Trim();
        service.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        service.Price = price;
        service.IsActive = IsActive;

        await _database.SaveExtraServiceAsync(service);
        await LoadAsync();
        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedService is null)
            return;

        await _database.DeleteExtraServiceAsync(SelectedService);
        await LoadAsync();
        ClearForm();
    }
}
