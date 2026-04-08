using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class ReportsPage : ContentPage
{
    private readonly ReportsViewModel _vm;

    public ReportsPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<ReportsViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
