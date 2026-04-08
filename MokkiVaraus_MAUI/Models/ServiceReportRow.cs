namespace MokkiVaraus_MAUI.Models;

public class ServiceReportRow
{
    public string AreaName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public DateTime BookingDate { get; set; }
}
