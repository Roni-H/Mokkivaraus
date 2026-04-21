using SQLite;

namespace MokkiVaraus_MAUI.Models;

[Table("ExtraServices")]
public class ExtraService
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int AreaId { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}
