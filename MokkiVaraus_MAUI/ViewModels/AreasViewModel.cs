using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class AreasViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private bool _isBusy;
    private Area? _selectedArea;
    private string _name = string.Empty;
    private string _description = string.Empty;

    public AreasViewModel(AppDatabase database)
    {
        _database = database;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedArea is not null);
        NewCommand = new RelayCommand(ClearForm);
    }

    public ObservableCollection<Area> Areas { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public RelayCommand NewCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public Area? SelectedArea
    {
        get => _selectedArea;
        set
        {
            if (SetProperty(ref _selectedArea, value))
            {
                LoadSelected();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }

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
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadSelected()
    {
        if (SelectedArea is null)
        {
            ClearForm();
            return;
        }

        Name = SelectedArea.Name;
        Description = SelectedArea.Description ?? string.Empty;
    }

    private void ClearForm()
    {
        SelectedArea = null;
        Name = string.Empty;
        Description = string.Empty;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        var area = SelectedArea ?? new Area();
        area.Name = Name.Trim();
        area.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();

        await _database.SaveAreaAsync(area);
        await LoadAsync();
        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedArea is null)
            return;

        await _database.DeleteAreaAsync(SelectedArea);
        await LoadAsync();
        ClearForm();
    }
}
