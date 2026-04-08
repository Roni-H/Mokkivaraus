using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class InvoicesPage : ContentPage
{
    private readonly InvoicesViewModel _vm;

    public InvoicesPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<InvoicesViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
