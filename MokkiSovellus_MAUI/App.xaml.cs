using MokkiSovellus_MAUI.Views;

namespace MokkiSovellus_MAUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = Services.GetRequiredService<AppShell>();
        return new Window(shell);
    }
}