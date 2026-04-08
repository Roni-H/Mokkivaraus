using SQLite;

namespace MokkiSovellus_MAUI.Models;

public class Area
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";
}