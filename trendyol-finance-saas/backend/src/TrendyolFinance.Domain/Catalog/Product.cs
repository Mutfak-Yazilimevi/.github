using TrendyolFinance.Domain.Common;

namespace TrendyolFinance.Domain.Catalog;

/// <summary>Trendyol ürünü (SKU/barkod bazında). Kâr, fiyat erimesi ve ölü stok analizinin nesnesi.</summary>
public class Product : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Guid SellerAccountId { get; set; }

    public required string Barcode { get; set; }
    public string? StockCode { get; set; }
    public required string Title { get; set; }

    public string? CategoryName { get; set; }
    public int? TrendyolCategoryId { get; set; }
    public string? Brand { get; set; }

    public decimal CurrentListPrice { get; set; }
    public decimal CurrentSalePrice { get; set; }
    public int CurrentStock { get; set; }

    public DateTimeOffset? LastSoldAt { get; set; }

    public ICollection<ProductCost> Costs { get; set; } = new List<ProductCost>();
    public ICollection<PriceSnapshot> PriceSnapshots { get; set; } = new List<PriceSnapshot>();
}

/// <summary>
/// Alış maliyeti (COGS) — Trendyol VERMEZ; kullanıcı manuel/Excel/muhasebe ile girer.
/// Tarih aralıklı (effective-dated): satış anındaki maliyetle doğru geçmiş kâr hesaplanır.
/// </summary>
public class ProductCost : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal VatRate { get; set; }          // % (örn. 20)
    public decimal PackagingCost { get; set; }
    public decimal OtherCost { get; set; }

    public CostSource Source { get; set; } = CostSource.Manual;

    /// <summary>Verilen tarihteki toplam birim maliyet.</summary>
    public decimal TotalUnitCost => PurchasePrice + PackagingCost + OtherCost;
}

public enum CostSource
{
    Manual = 0,
    ExcelImport = 1,
    AccountingIntegration = 2
}

/// <summary>Günlük fiyat fotoğrafı — enflasyona göre reel fiyat erimesi (F4) için.</summary>
public class PriceSnapshot : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }

    public DateOnly CapturedOn { get; set; }
    public decimal ListPrice { get; set; }
    public decimal SalePrice { get; set; }
    public string Currency { get; set; } = "TRY";
}
