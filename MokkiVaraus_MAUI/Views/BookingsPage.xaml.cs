using Microsoft.Extensions.DependencyInjection;
using MokkiVaraus_MAUI.ViewModels;

namespace MokkiVaraus_MAUI.Views;

public partial class BookingsPage : ContentPage
{
    private readonly BookingsViewModel _vm;

    public BookingsPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<BookingsViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
