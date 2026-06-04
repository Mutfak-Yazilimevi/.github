using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>
/// Satış (sevkiyat) kaydı. Oluşturulduğunda mamul stoğundan düşülür; satış anındaki birim maliyet
/// (COGS) dondurulur, böylece gelir ve kâr geçmişe dönük tutarlı raporlanır.
/// </summary>
public class SalesOrder : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>Birim satış fiyatı (KDV hariç, para birimi).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Satış anındaki birim maliyet (mamul ağırlıklı ort. maliyeti) — COGS.</summary>
    public decimal UnitCost { get; set; }

    public string? CustomerName { get; set; }
    public string? Notes { get; set; }

    /// <summary>Toplam ciro = adet × birim fiyat.</summary>
    public decimal Revenue => Quantity * UnitPrice;

    /// <summary>Toplam kâr = adet × (birim fiyat − birim maliyet).</summary>
    public decimal Profit => Quantity * (UnitPrice - UnitCost);
}
