using SQLite;

namespace MokkiVaraus_MAUI.Models;

[Table("BookingExtraServices")]
public class BookingExtraService
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int BookingId { get; set; }

    [Indexed]
    public int ExtraServiceId { get; set; }

    public int Quantity { get; set; } = 1;

    public decimal UnitPrice { get; set; }
}
