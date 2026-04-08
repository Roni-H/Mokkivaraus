using Microsoft.Extensions.Logging;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Services;
using MokkiVaraus_MAUI.ViewModels;
using MokkiVaraus_MAUI.Views;

namespace MokkiVaraus_MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<ReportService>();
        builder.Services.AddSingleton<InvoiceService>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<AreasViewModel>();
        builder.Services.AddSingleton<ServicesViewModel>();
        builder.Services.AddSingleton<CustomersViewModel>();
        builder.Services.AddSingleton<CottagesViewModel>();
        builder.Services.AddSingleton<BookingsViewModel>();
        builder.Services.AddSingleton<InvoicesViewModel>();
        builder.Services.AddSingleton<ReportsViewModel>();

        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<AreasPage>();
        builder.Services.AddSingleton<ServicesPage>();
        builder.Services.AddSingleton<CustomersPage>();
        builder.Services.AddSingleton<CottagesPage>();
        builder.Services.AddSingleton<BookingsPage>();
        builder.Services.AddSingleton<InvoicesPage>();
        builder.Services.AddSingleton<ReportsPage>();

        return builder.Build();
    }
}
