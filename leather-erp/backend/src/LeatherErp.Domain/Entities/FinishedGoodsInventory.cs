using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>Eldeki mamul (bitmiş ürün) stoğu. Üretim arttıkça, satış/sevkiyat azalttıkça güncellenir.</summary>
public class FinishedGoodsInventory : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Eldeki adet.</summary>
    public int QuantityOnHand { get; set; }

    /// <summary>Ağırlıklı ortalama birim maliyet (para birimi). Stok değerinin temelidir.</summary>
    public decimal AverageUnitCost { get; set; }

    /// <summary>Toplam stok değeri = adet × ortalama birim maliyet.</summary>
    public decimal TotalValue => QuantityOnHand * AverageUnitCost;
}
