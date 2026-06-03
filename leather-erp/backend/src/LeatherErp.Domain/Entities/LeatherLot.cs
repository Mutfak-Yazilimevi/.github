using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>
/// Tek bir deri alım partisi (lot). Miktarlar kanonik dm² olarak tutulur.
/// Üretimde tüketim FIFO (en eski lot önce) mantığıyla bu lotlardan düşülür.
/// </summary>
public class LeatherLot : BaseEntity
{
    public Guid LeatherTypeId { get; set; }
    public LeatherType LeatherType { get; set; } = null!;

    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    /// <summary>Lot/parti numarası (opsiyonel, izlenebilirlik için).</summary>
    public string? LotNumber { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    /// <summary>Giriş miktarı (dm²).</summary>
    public decimal QuantityDm2 { get; set; }

    /// <summary>Kalan miktar (dm²). Üretim tükettikçe azalır.</summary>
    public decimal RemainingDm2 { get; set; }

    /// <summary>Birim alış maliyeti (para birimi / dm²).</summary>
    public decimal UnitCostPerDm2 { get; set; }

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
}
