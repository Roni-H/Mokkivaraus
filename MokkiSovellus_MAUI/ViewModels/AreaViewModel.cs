using System.Collections.ObjectModel;
using System.Windows.Input;
using MokkiSovellus_MAUI.Models;
using MokkiSovellus_MAUI.Services;

namespace MokkiSovellus_MAUI.ViewModels;

public class AreaViewModel : BaseViewModel
{
    private readonly AreaService _service;

    public ObservableCollection<Area> Areas { get; } = new();

    private Area? _selectedArea;
    public Area? SelectedArea
    {
        get => _selectedArea;
        set
        {
            if (SetProperty(ref _selectedArea, value))
                AreaName = value?.Name ?? "";
        }
    }

    private string _areaName = "";
    public string AreaName
    {
        get => _areaName;
        set => SetProperty(ref _areaName, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }

    public AreaViewModel(AreaService service)
    {
        _service = service;

        LoadCommand = new Command(Load);
        SaveCommand = new Command(Save);
        DeleteCommand = new Command(Delete);
        ClearCommand = new Command(Clear);

        Load();
    }

    public void Load()
    {
        Areas.Clear();
        foreach (var area in _service.GetAll())
            Areas.Add(area);
    }

    private void Save()
    {
        _service.Save(new Area
        {
            Id = SelectedArea?.Id ?? 0,
            Name = AreaName.Trim()
        });

        Clear();
        Load();
    }

    private void Delete()
    {
        if (SelectedArea is null)
            return;

        _service.Delete(SelectedArea);
        Clear();
        Load();
    }

    private void Clear()
    {
        SelectedArea = null;
        AreaName = "";
    }
}