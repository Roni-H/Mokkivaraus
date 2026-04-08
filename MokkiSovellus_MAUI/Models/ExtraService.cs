using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class ExtraService
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public double Price { get; set; }
}