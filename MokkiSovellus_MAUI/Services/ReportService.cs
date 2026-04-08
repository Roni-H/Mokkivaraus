using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class ReportService
{
    private readonly AppDatabase _db;
    public ReportService(AppDatabase db) => _db = db;

    public List<Booking> GetBookingsByDateRange(DateTime from, DateTime to)
    {
        return _db.Connection.Table<Booking>()
            .Where(b => b.StartDate >= from && b.EndDate <= to)
            .ToList();
    }
}