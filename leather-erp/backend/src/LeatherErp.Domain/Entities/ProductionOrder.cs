using LeatherErp.Domain.Common;
using LeatherErp.Domain.Enums;

namespace LeatherErp.Domain.Entities;

/// <summary>
/// Üretim emri: bir üründen kaç adet üretildiğini temsil eder. Onaylandığında deri stoğundan
/// (FIFO) düşülür ve mamul stoğa eklenir; gerçekleşen birim maliyet anlık olarak dondurulur.
/// </summary>
public class ProductionOrder : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public ProductionStatus Status { get; set; } = ProductionStatus.Draft;

    /// <summary>Onay anında hesaplanan birim maliyet (para birimi). Sonraki fiyat değişimlerinden bağımsızdır.</summary>
    public decimal UnitCostSnapshot { get; set; }

    /// <summary>Toplam tüketilen brüt deri (dm²) = net × (1 + fire) × adet.</summary>
    public decimal TotalLeatherConsumedDm2 { get; set; }

    public string? Notes { get; set; }
}
