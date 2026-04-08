using MokkiSovellus_MAUI.Models;
using SQLite;

namespace MokkiSovellus_MAUI.Data;

public class AppDatabase
{
    public SQLiteConnection Connection { get; }

    public AppDatabase(string dbPath)
    {
        Connection = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);

        Connection.CreateTable<Area>();
        Connection.CreateTable<Cottage>();
        Connection.CreateTable<Customer>();
        Connection.CreateTable<ExtraService>();
        Connection.CreateTable<Booking>();
        Connection.CreateTable<BookingExtraService>();
        Connection.CreateTable<Invoice>();

        SeedIfEmpty();
    }

    private void SeedIfEmpty()
    {
        if (!Connection.Table<Area>().Any())
        {
            Connection.InsertAll(new[]
            {
                new Area { Name = "Ruka" },
                new Area { Name = "Tahko" },
                new Area { Name = "Ylläs" },
                new Area { Name = "Levi" },
                new Area { Name = "Pyhä" },
                new Area { Name = "Iso-Syöte" },
                new Area { Name = "Vuokatti" },
                new Area { Name = "Himos" }
            });
        }

        if (!Connection.Table<ExtraService>().Any())
        {
            Connection.InsertAll(new[]
            {
                new ExtraService { Name = "Porosafari", Price = 79 },
                new ExtraService { Name = "Koiravaljakko", Price = 89 },
                new ExtraService { Name = "Vesiskootteri", Price = 120 },
                new ExtraService { Name = "Hevosajelu", Price = 69 },
                new ExtraService { Name = "Airsoft", Price = 45 }
            });
        }
    }
}