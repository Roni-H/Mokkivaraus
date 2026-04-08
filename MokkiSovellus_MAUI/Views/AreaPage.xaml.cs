using MokkiSovellus_MAUI.ViewModels;

namespace MokkiSovellus_MAUI.Views;

public partial class AreaPage : ContentPage
{
    public AreaPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<AreaViewModel>();
    }
}