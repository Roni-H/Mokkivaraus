using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class Booking
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CottageId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Cancelled { get; set; }
    public string Notes { get; set; } = "";
}