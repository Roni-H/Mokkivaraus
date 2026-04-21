using SQLite;

namespace MokkiVaraus_MAUI.Models;

public enum InvoiceDeliveryMethod
{
    Paper = 0,
    Email = 1
}

[Table("Invoices")]
public class Invoice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int BookingId { get; set; }

    [Indexed]
    public int CustomerId { get; set; }

    [Indexed(Unique = true)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; }

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public InvoiceDeliveryMethod DeliveryMethod { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? Notes { get; set; }
}
