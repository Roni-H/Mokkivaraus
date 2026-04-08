using Microsoft.Extensions.Logging;
using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Services;
using MokkiSovellus_MAUI.ViewModels;
using MokkiSovellus_MAUI.Views;

namespace MokkiSovellus_MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "village.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));

        builder.Services.AddSingleton<AreaService>();
        builder.Services.AddSingleton<CottageService>();
        builder.Services.AddSingleton<CustomerService>();
        builder.Services.AddSingleton<ExtraServiceService>();
        builder.Services.AddSingleton<BookingService>();
        builder.Services.AddSingleton<InvoiceService>();
        builder.Services.AddSingleton<ReportService>();

        builder.Services.AddTransient<AreaViewModel>();
        builder.Services.AddTransient<AreaPage>();

        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}