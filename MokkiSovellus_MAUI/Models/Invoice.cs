using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class Invoice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int BookingId { get; set; }
    public double Total { get; set; }
    public bool Paid { get; set; }
    public string DeliveryMethod { get; set; } = "Email";
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ReminderSentAt { get; set; }
}