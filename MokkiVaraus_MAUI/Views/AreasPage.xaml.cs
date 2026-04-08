using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class AreasPage : ContentPage
{
    private readonly AreasViewModel _vm;

    public AreasPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<AreasViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
