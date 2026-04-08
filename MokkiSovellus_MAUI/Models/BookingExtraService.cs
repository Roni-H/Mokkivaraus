using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class BookingExtraService
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int BookingId { get; set; }
    public int ExtraServiceId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
}