using LeatherErp.Domain.Common;
using LeatherErp.Domain.Enums;

namespace LeatherErp.Domain.Entities;

/// <summary>Bir deri lotu üzerindeki stok hareketi (giriş/çıkış/fire/düzeltme). Denetim izi sağlar.</summary>
public class StockMovement : BaseEntity
{
    public Guid LeatherLotId { get; set; }
    public LeatherLot LeatherLot { get; set; } = null!;

    public MovementDirection Direction { get; set; }

    /// <summary>Hareket miktarı (dm²), her zaman pozitif; yön <see cref="Direction"/> ile belirlenir.</summary>
    public decimal QuantityDm2 { get; set; }

    public DateTime MovementDate { get; set; } = DateTime.UtcNow;

    /// <summary>Açıklama/sebep (örn. "Üretim emri #1234").</summary>
    public string? Reason { get; set; }

    /// <summary>Hareketi tetikleyen üretim emri (varsa).</summary>
    public Guid? ProductionOrderId { get; set; }
}
