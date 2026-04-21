using SQLite;

namespace MokkiVaraus_MAUI.Models;

public enum BookingStatus
{
    Tentative = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}

[Table("Bookings")]
public class Booking
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CottageId { get; set; }

    [Indexed]
    public int CustomerId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Tentative;

    public string? Notes { get; set; }
}
