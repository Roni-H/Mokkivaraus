using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class Cottage
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public int AreaId { get; set; }
    public double PricePerNight { get; set; }
    public string Description { get; set; } = "";
    public string Features { get; set; } = "";
    public int MaxPersons { get; set; }
}