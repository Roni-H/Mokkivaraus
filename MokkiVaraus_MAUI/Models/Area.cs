using SQLite;

namespace MokkiVaraus_MAUI.Models;

[Table("Areas")]
public class Area
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed(Unique = true)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public override string ToString() => Name;
}
