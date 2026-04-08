using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class CottagesPage : ContentPage
{
    private readonly CottagesViewModel _vm;

    public CottagesPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<CottagesViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
