using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>
/// Ürün reçetesi (BOM): bir adet ürün üretmek için gereken deri, işçilik, genel gider ve fire oranı.
/// Maliyet hesabının girdisidir.
/// </summary>
public class ProductRecipe : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Kullanılan deri tipi.</summary>
    public Guid LeatherTypeId { get; set; }
    public LeatherType LeatherType { get; set; } = null!;

    /// <summary>Bir ürün için net (fire hariç) kullanılan deri miktarı (dm²).</summary>
    public decimal NetLeatherDm2 { get; set; }

    /// <summary>Fire oranı (0–1 arası ondalık; örn. 0.15 = %15). Net deri üzerine eklenir.</summary>
    public decimal WasteRate { get; set; }

    /// <summary>Birim işçilik maliyeti (para birimi).</summary>
    public decimal LaborCost { get; set; }

    /// <summary>Birim genel gider (para birimi).</summary>
    public decimal OverheadCost { get; set; }
}
