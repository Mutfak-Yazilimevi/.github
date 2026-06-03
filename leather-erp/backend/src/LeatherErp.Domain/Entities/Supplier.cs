using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>Deri tedarikçisi.</summary>
public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }

    public ICollection<LeatherLot> Lots { get; set; } = new List<LeatherLot>();
}
