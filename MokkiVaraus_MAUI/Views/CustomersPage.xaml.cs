using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class CustomersPage : ContentPage
{
    private readonly CustomersViewModel _vm;

    public CustomersPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<CustomersViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
