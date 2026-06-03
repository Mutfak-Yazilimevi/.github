using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>Üretilen mamul ürün (örn. cüzdan, çanta, kemer). Maliyet/fiyat hesabı reçeteden gelir.</summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Ürün reçetesi (malzeme + işçilik + fire). Bire-bir ilişki.</summary>
    public ProductRecipe? Recipe { get; set; }

    public FinishedGoodsInventory? Inventory { get; set; }
}
