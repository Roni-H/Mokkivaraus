using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class BookingService
{
    private readonly AppDatabase _db;
    public BookingService(AppDatabase db) => _db = db;

    public List<Booking> GetAll() => _db.Connection.Table<Booking>().ToList();

    public void Save(Booking booking)
    {
        if (booking.Id == 0) _db.Connection.Insert(booking);
        else _db.Connection.Update(booking);
    }

    public void Delete(Booking booking) => _db.Connection.Delete(booking);
}