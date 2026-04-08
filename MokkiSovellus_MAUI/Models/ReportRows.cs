namespace MokkiSovellus_MAUI.Models;

public class BookingReportRow
{
    public string AreaName { get; set; } = "";
    public string CottageName { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Total { get; set; }
}

public class ServiceReportRow
{
    public string AreaName { get; set; } = "";
    public string ServiceName { get; set; } = "";
    public int Quantity { get; set; }
    public double Total { get; set; }
}