using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class ServicesPage : ContentPage
{
    private readonly ServicesViewModel _vm;

    public ServicesPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<ServicesViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
