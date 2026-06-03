namespace LeatherErp.Application.Dtos;

/// <summary>Birim maliyet hesabı girdisi.</summary>
public record CostInput
{
    /// <summary>Net (fire hariç) kullanılan deri miktarı (dm²).</summary>
    public decimal NetLeatherDm2 { get; init; }

    /// <summary>Fire oranı (0–1; örn. 0.15 = %15).</summary>
    public decimal WasteRate { get; init; }

    /// <summary>Deri birim maliyeti (para birimi / dm²).</summary>
    public decimal UnitCostPerDm2 { get; init; }

    /// <summary>Birim işçilik maliyeti.</summary>
    public decimal LaborCost { get; init; }

    /// <summary>Birim genel gider.</summary>
    public decimal OverheadCost { get; init; }
}

/// <summary>Birim maliyet hesabının şeffaf kırılımı.</summary>
public record CostBreakdown
{
    /// <summary>Fire dahil tüketilen brüt deri (dm²) = net × (1 + fire).</summary>
    public decimal GrossLeatherDm2 { get; init; }

    /// <summary>Net derinin malzeme maliyeti (fire hariç).</summary>
    public decimal NetMaterialCost { get; init; }

    /// <summary>Yalnızca fireden kaynaklanan ek malzeme maliyeti.</summary>
    public decimal WasteCost { get; init; }

    /// <summary>Toplam malzeme maliyeti (fire dahil) = NetMaterialCost + WasteCost.</summary>
    public decimal MaterialCost { get; init; }

    public decimal LaborCost { get; init; }

    public decimal OverheadCost { get; init; }

    /// <summary>Birim toplam maliyet = MaterialCost + LaborCost + OverheadCost.</summary>
    public decimal UnitCost { get; init; }
}
