using MokkiSovellus_MAUI.Views;
using static System.Net.Mime.MediaTypeNames;

namespace MokkiSovellus_MAUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;
        MainPage = serviceProvider.GetRequiredService<AppShell>();
    }
}