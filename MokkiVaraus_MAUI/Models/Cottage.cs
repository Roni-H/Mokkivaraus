using SQLite;

namespace MokkiVaraus_MAUI.Models;

[Table("Cottages")]
public class Cottage
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int AreaId { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Capacity { get; set; }

    public decimal NightlyPrice { get; set; }

    public string? Equipment { get; set; }

    public bool IsActive { get; set; } = true;
}
