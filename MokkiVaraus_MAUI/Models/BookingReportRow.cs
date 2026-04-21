namespace MokkiVaraus_MAUI.Models;

public class BookingReportRow
{
    public int BookingId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string CottageName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
