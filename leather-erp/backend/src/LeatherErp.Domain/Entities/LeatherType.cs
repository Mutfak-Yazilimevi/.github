using LeatherErp.Domain.Common;

namespace LeatherErp.Domain.Entities;

/// <summary>Deri tipi tanımı (örn. "Vaketa - Kahverengi - 1.8mm"). Stok lotları bu tipe bağlanır.</summary>
public class LeatherType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Deri türü/cinsi (vaketa, nubuk, süet, rugan vb.).</summary>
    public string? Kind { get; set; }

    public string? Color { get; set; }

    /// <summary>Kalınlık (mm).</summary>
    public decimal? ThicknessMm { get; set; }

    /// <summary>Bu tip için düşük stok uyarı eşiği (dm²). Altına düşünce uyarı verilir.</summary>
    public decimal LowStockThresholdDm2 { get; set; }

    public ICollection<LeatherLot> Lots { get; set; } = new List<LeatherLot>();
}
