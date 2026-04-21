using SQLite;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.Data;

public sealed class AppDatabase
{
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _db;
    private bool _initialized;

    private SQLiteAsyncConnection Db => _db ?? throw new InvalidOperationException("Tietokantaa ei ole alustettu.");

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized)
                return;

            var path = Path.Combine(FileSystem.AppDataDirectory, "mokki_varausjärjestelmä.db3");
            _db = new SQLiteAsyncConnection(path);

            await _db.ExecuteAsync("PRAGMA foreign_keys = ON;");

            await _db.CreateTableAsync<Area>();
            await _db.CreateTableAsync<Cottage>();
            await _db.CreateTableAsync<ExtraService>();
            await _db.CreateTableAsync<Customer>();
            await _db.CreateTableAsync<Booking>();
            await _db.CreateTableAsync<BookingExtraService>();
            await _db.CreateTableAsync<Invoice>();

            if (await _db.Table<Area>().CountAsync() == 0)
                await SeedAsync();

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task SeedAsync()
    {
        var areas = new[]
        {
            new Area { Name = "Ruka", Description = "Laskettelukeskus ja aktiivinen lomakohde." },
            new Area { Name = "Tahko", Description = "Ympärivuotinen vapaa-ajan alue." },
            new Area { Name = "Ylläs", Description = "Luonto, rinteet ja hiljaisuus." },
            new Area { Name = "Levi", Description = "Korkean kysynnän sesonkialue." }
        };

        foreach (var area in areas)
            await Db.InsertAsync(area);

        var cottages = new[]
        {
            new Cottage { AreaId = 1, Name = "Rukan Huvila", Capacity = 8, NightlyPrice = 245m, Equipment = "Sauna, takka, wifi", Description = "Tyylikäs mökki lähellä rinteitä.", IsActive = true },
            new Cottage { AreaId = 2, Name = "Tahkon Kelohuvila", Capacity = 6, NightlyPrice = 210m, Equipment = "Sauna, palju, wifi", Description = "Hyvä vaihtoehto perheille.", IsActive = true },
            new Cottage { AreaId = 3, Name = "Ylläksen Lake Lodge", Capacity = 10, NightlyPrice = 290m, Equipment = "Sauna, kota, lataus", Description = "Tilava mökki suuremmalle ryhmälle.", IsActive = true },
            new Cottage { AreaId = 4, Name = "Levi Peak Cabin", Capacity = 5, NightlyPrice = 320m, Equipment = "Sauna, wifi, rinnekuljetus", Description = "Premium-kohde sesonkiasiakkaille.", IsActive = true }
        };

        foreach (var cottage in cottages)
            await Db.InsertAsync(cottage);

        var services = new[]
        {
            new ExtraService { AreaId = 1, Name = "Porosafari", Price = 79m, Description = "Perheille sopiva ohjelmapalvelu.", IsActive = true },
            new ExtraService { AreaId = 1, Name = "Koiravaljakkoajelu", Price = 119m, Description = "Elämyksellinen talviajelu.", IsActive = true },
            new ExtraService { AreaId = 2, Name = "Vesiskootteriajelu", Price = 89m, Description = "Kesäaktiviteetti järvimaisemissa.", IsActive = true },
            new ExtraService { AreaId = 3, Name = "Hevosajelu", Price = 69m, Description = "Rauhallinen retki luonnossa.", IsActive = true },
            new ExtraService { AreaId = 4, Name = "Airsoft-paketti", Price = 149m, Description = "Ryhmille sopiva ohjelma.", IsActive = true }
        };

        foreach (var service in services)
            await Db.InsertAsync(service);

        var customers = new[]
        {
            new Customer { FirstName = "Aino", LastName = "Korhonen", Email = "aino@example.com", PhoneNumber = "0401234567", Address = "Oulu" },
            new Customer { FirstName = "Mikko", LastName = "Laine", Email = "mikko@example.com", PhoneNumber = "0407654321", Address = "Helsinki" },
            new Customer { FirstName = "Sari", LastName = "Mäkelä", Email = "sari@example.com", PhoneNumber = "0501122334", Address = "Tampere" }
        };

        foreach (var customer in customers)
            await Db.InsertAsync(customer);

        var bookings = new[]
        {
            new Booking
            {
                CottageId = 1,
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(10),
                Status = BookingStatus.Confirmed,
                Notes = "Saapuminen illalla."
            },
            new Booking
            {
                CottageId = 2,
                CustomerId = 2,
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(18),
                Status = BookingStatus.Confirmed,
                Notes = "Perheloma."
            },
            new Booking
            {
                CottageId = 4,
                CustomerId = 3,
                StartDate = DateTime.Today.AddDays(21),
                EndDate = DateTime.Today.AddDays(25),
                Status = BookingStatus.Tentative,
                Notes = "Odottaa vahvistusta."
            }
        };

        foreach (var booking in bookings)
            await Db.InsertAsync(booking);

        await Db.InsertAsync(new BookingExtraService { BookingId = 1, ExtraServiceId = 1, Quantity = 2, UnitPrice = 79m });
        await Db.InsertAsync(new BookingExtraService { BookingId = 2, ExtraServiceId = 3, Quantity = 1, UnitPrice = 89m });

        await Db.InsertAsync(new Invoice
        {
            BookingId = 1,
            CustomerId = 1,
            InvoiceNumber = "LASKU-2026-0001",
            IssuedAt = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),
            Amount = 245m * 3 + 79m * 2,
            DeliveryMethod = InvoiceDeliveryMethod.Email,
            IsPaid = false
        });
    }

    public async Task<List<Area>> GetAreasAsync()
    {
        await InitializeAsync();
        return await Db.Table<Area>().OrderBy(a => a.Name).ToListAsync();
    }

    public async Task<Area?> GetAreaByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<Area>().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> SaveAreaAsync(Area area)
    {
        await InitializeAsync();
        return area.Id == 0 ? await Db.InsertAsync(area) : await Db.UpdateAsync(area);
    }

    public async Task<int> DeleteAreaAsync(Area area)
    {
        await InitializeAsync();
        return await Db.DeleteAsync(area);
    }

    public async Task<List<Cottage>> GetCottagesAsync()
    {
        await InitializeAsync();
        return await Db.Table<Cottage>().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Cottage?> GetCottageByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<Cottage>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> SaveCottageAsync(Cottage cottage)
    {
        await InitializeAsync();

        if (cottage.Id == 0)
        {
            await EnsureNoOverlapAsync(cottage.AreaId, cottage.Id, DateTime.MinValue, DateTime.MinValue);
            return await Db.InsertAsync(cottage);
        }

        return await Db.UpdateAsync(cottage);
    }

    public async Task<int> DeleteCottageAsync(Cottage cottage)
    {
        await InitializeAsync();
        return await Db.DeleteAsync(cottage);
    }

    public async Task<List<ExtraService>> GetExtraServicesAsync()
    {
        await InitializeAsync();
        return await Db.Table<ExtraService>().OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<ExtraService?> GetExtraServiceByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<ExtraService>().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<int> SaveExtraServiceAsync(ExtraService service)
    {
        await InitializeAsync();
        return service.Id == 0 ? await Db.InsertAsync(service) : await Db.UpdateAsync(service);
    }

    public async Task<int> DeleteExtraServiceAsync(ExtraService service)
    {
        await InitializeAsync();
        return await Db.DeleteAsync(service);
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        await InitializeAsync();
        return await Db.Table<Customer>().OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<Customer>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> SaveCustomerAsync(Customer customer)
    {
        await InitializeAsync();
        return customer.Id == 0 ? await Db.InsertAsync(customer) : await Db.UpdateAsync(customer);
    }

    public async Task<int> DeleteCustomerAsync(Customer customer)
    {
        await InitializeAsync();
        return await Db.DeleteAsync(customer);
    }

    public async Task<List<Booking>> GetBookingsAsync()
    {
        await InitializeAsync();
        return await Db.Table<Booking>().OrderByDescending(b => b.StartDate).ToListAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<Booking>().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<int> SaveBookingAsync(Booking booking)
    {
        await InitializeAsync();
        await EnsureNoOverlapAsync(booking.CottageId, booking.Id, booking.StartDate, booking.EndDate);

        return booking.Id == 0 ? await Db.InsertAsync(booking) : await Db.UpdateAsync(booking);
    }

    public async Task<int> DeleteBookingAsync(Booking booking)
    {
        await InitializeAsync();
        return await Db.DeleteAsync(booking);
    }

    public async Task<List<BookingExtraService>> GetBookingExtraServicesAsync()
    {
        await InitializeAsync();
        return await Db.Table<BookingExtraService>().ToListAsync();
    }

    public async Task<int> SaveBookingExtraServiceAsync(BookingExtraService row)
    {
        await InitializeAsync();
        return row.Id == 0 ? await Db.InsertAsync(row) : await Db.UpdateAsync(row);
    }

    public async Task<List<Invoice>> GetInvoicesAsync()
    {
        await InitializeAsync();
        return await Db.Table<Invoice>().OrderByDescending(i => i.IssuedAt).ToListAsync();
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        await InitializeAsync();
        return await Db.Table<Invoice>().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<int> SaveInvoiceAsync(Invoice invoice)
    {
        await InitializeAsync();
        return invoice.Id == 0 ? await Db.InsertAsync(invoice) : await Db.UpdateAsync(invoice);
    }

    public async Task<int> MarkInvoiceAsPaidAsync(int invoiceId)
    {
        await InitializeAsync();
        var invoice = await GetInvoiceByIdAsync(invoiceId) ?? throw new InvalidOperationException("Laskua ei löytynyt.");
        invoice.IsPaid = true;
        invoice.PaidAt = DateTime.Now;
        return await Db.UpdateAsync(invoice);
    }

    public async Task<List<Cottage>> SearchAvailableCottagesAsync(
        int? areaId = null,
        decimal? maxNightlyPrice = null,
        string? requiredEquipment = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        await InitializeAsync();

        var cottages = await GetCottagesAsync();

        if (areaId.HasValue)
            cottages = cottages.Where(c => c.AreaId == areaId.Value).ToList();

        if (maxNightlyPrice.HasValue)
            cottages = cottages.Where(c => c.NightlyPrice <= maxNightlyPrice.Value).ToList();

        if (!string.IsNullOrWhiteSpace(requiredEquipment))
        {
            var tokens = requiredEquipment
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToLowerInvariant())
                .ToList();

            cottages = cottages.Where(c =>
            {
                var equipment = (c.Equipment ?? string.Empty).ToLowerInvariant();
                return tokens.All(token => equipment.Contains(token));
            }).ToList();
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            var bookings = await GetBookingsAsync();
            cottages = cottages.Where(c =>
                !bookings.Any(b =>
                    b.CottageId == c.Id &&
                    b.Status != BookingStatus.Cancelled &&
                    b.StartDate < endDate.Value &&
                    b.EndDate > startDate.Value)).ToList();
        }

        return cottages;
    }

    private async Task EnsureNoOverlapAsync(int cottageId, int bookingId, DateTime startDate, DateTime endDate)
    {
        if (startDate == DateTime.MinValue && endDate == DateTime.MinValue)
            return;

        var bookings = await GetBookingsAsync();
        var overlap = bookings.Any(b =>
            b.CottageId == cottageId &&
            b.Id != bookingId &&
            b.Status != BookingStatus.Cancelled &&
            b.StartDate < endDate &&
            b.EndDate > startDate);

        if (overlap)
            throw new InvalidOperationException("Tälle mökille on jo päällekkäinen varaus valitulla ajalla.");
    }

    // ---- Helpers for locating and manipulating the DB file and running raw SQL ----

    /// <summary>
    /// Returns the full path to the app database file on the device.
    /// Copy/paste this path into File Explorer, Terminal, or use platform tools to pull it.
    /// </summary>
    public string GetDatabaseFilePath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, "mokki_varausjärjestelmä.db3");
    }

    /// <summary>
    /// Execute a raw non-query SQL statement (INSERT/UPDATE/DELETE/DDL).
    /// Returns affected row count when supported.
    /// </summary>
    public async Task<int> ExecuteNonQueryAsync(string sql, params object[] args)
    {
        await InitializeAsync();
        return await Db.ExecuteAsync(sql, args);
    }

    /// <summary>
    /// Execute an external SQL script (multiple statements separated by ';').
    /// This runs inside a transaction.
    /// </summary>
    public async Task ExecuteSqlScriptAsync(string sqlScript)
    {
        await InitializeAsync();

        var statements = sqlScript
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        await Db.RunInTransactionAsync(conn =>
        {
            foreach (var stmt in statements)
            {
                // conn is synchronous SQLiteConnection (sqlite-net)
                conn.Execute(stmt);
            }
        });
    }

    /// <summary>
    /// Copy the database file to a destination path accessible from your machine.
    /// Note: destination must be writable by the app; on Android you may prefer to copy to external cache or use the Android Device File Explorer.
    /// </summary>
    public Task ExportDatabaseAsync(string destinationFullPath, bool overwrite = true)
    {
        var source = GetDatabaseFilePath();
        if (!File.Exists(source))
            throw new FileNotFoundException("Database file not found.", source);

        File.Copy(source, destinationFullPath, overwrite);
        return Task.CompletedTask;
    }


}
