using SQLite;

namespace MokkiVaraus_MAUI.Models;

[Table("Customers")]
public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string FirstName { get; set; } = string.Empty;

    [NotNull]
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string FullName => $"{LastName} {FirstName}";
}
