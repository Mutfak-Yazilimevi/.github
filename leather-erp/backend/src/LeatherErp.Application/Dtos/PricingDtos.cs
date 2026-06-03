namespace LeatherErp.Application.Dtos;

/// <summary>İleri yön fiyatlandırma girdisi: maliyetten satış fiyatına.</summary>
public record PricingInput
{
    /// <summary>Birim maliyet (para birimi).</summary>
    public decimal UnitCost { get; init; }

    /// <summary>Kâr marjı (0–1; maliyet üzerine eklenen oran, markup).</summary>
    public decimal ProfitMargin { get; init; }

    /// <summary>KDV oranı (0–1).</summary>
    public decimal VatRate { get; init; }
}

/// <summary>Fiyatlandırma sonucu kırılımı.</summary>
public record PricingResult
{
    public decimal UnitCost { get; init; }

    /// <summary>Kâr tutarı = UnitCost × ProfitMargin.</summary>
    public decimal ProfitAmount { get; init; }

    /// <summary>KDV hariç satış fiyatı = UnitCost × (1 + ProfitMargin).</summary>
    public decimal PriceBeforeTax { get; init; }

    /// <summary>KDV tutarı = PriceBeforeTax × VatRate.</summary>
    public decimal VatAmount { get; init; }

    /// <summary>KDV dahil satış fiyatı.</summary>
    public decimal SalePrice { get; init; }
}

/// <summary>Hedef satış fiyatından gerçekleşen kâr marjını çözen ters hesap girdisi.</summary>
public record ReversePricingInput
{
    public decimal UnitCost { get; init; }

    /// <summary>Hedeflenen KDV dahil satış fiyatı.</summary>
    public decimal TargetSalePrice { get; init; }

    public decimal VatRate { get; init; }
}
