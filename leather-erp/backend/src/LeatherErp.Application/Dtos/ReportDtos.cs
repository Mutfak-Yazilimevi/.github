namespace LeatherErp.Application.Dtos;

/// <summary>Bir ürünün maliyet + fiyat kırılımı (paylaşılan hesaplama çıktısı).</summary>
public record ProductCosting
{
    public Guid ProductId { get; init; }
    public decimal LeatherAvgUnitCostPerDm2 { get; init; }
    public decimal AvailableLeatherDm2 { get; init; }
    public CostBreakdown Cost { get; init; } = new();
    public PricingResult Price { get; init; } = new();
    public decimal AppliedProfitMargin { get; init; }
    public decimal AppliedVatRate { get; init; }
}

/// <summary>Panel/rapor için genel özet KPI'ları.</summary>
public record ReportSummary
{
    public decimal TotalLeatherStockDm2 { get; init; }
    public decimal LeatherStockValue { get; init; }
    public decimal FinishedGoodsValue { get; init; }
    public int FinishedGoodsUnits { get; init; }
    public int LowStockCount { get; init; }
    public int ProductionOrderCount { get; init; }
    public int UnitsProduced { get; init; }
}

/// <summary>Ürün bazında kârlılık satırı.</summary>
public record ProductProfitability
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitCost { get; init; }
    public decimal SalePrice { get; init; }
    /// <summary>Birim kâr (KDV hariç fiyat − maliyet).</summary>
    public decimal UnitProfit { get; init; }
    /// <summary>Kâr marjı (kâr / maliyet).</summary>
    public decimal ProfitMargin { get; init; }
    /// <summary>Reçetedeki deri için kaç adetlik üretim yapılabilir (eldeki stoğa göre).</summary>
    public int ProducibleUnits { get; init; }
}
