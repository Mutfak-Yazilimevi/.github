using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Application.Cogs;
using TrendyolFinance.Domain.Catalog;
using Xunit;

namespace TrendyolFinance.UnitTests;

public class RealPriceServiceTests
{
    [Fact]
    public void Enflasyon_fiyati_eritince_negatif_erime_dondurur()
    {
        var pid = Guid.NewGuid();
        var snaps = new List<PriceSnapshot>
        {
            new() { ProductId = pid, CapturedOn = new DateOnly(2026, 1, 1), SalePrice = 100m },
            new() { ProductId = pid, CapturedOn = new DateOnly(2026, 6, 1), SalePrice = 110m },
        };
        // TÜFE %50 artmış: 200 → 300. Fiyat sadece %10 artmış → reel olarak erimiş.
        var cpi = new Dictionary<int, decimal> { [202601] = 200m, [202606] = 300m };

        var r = new RealPriceService().Compute(snaps, cpi)!;

        Assert.Equal(100m, r.BasePrice);
        Assert.Equal(110m, r.CurrentNominal);
        // reel = 110 × (200/300) = 73.33 → baz 100'e göre ~ -%26.7
        Assert.True(r.ErosionPercent < 0);
        Assert.Equal(73.33m, r.CurrentReal);
    }
}

public class DeadStockServiceTests
{
    [Fact]
    public void Esik_ustu_satilmayan_stoklu_urun_listelenir()
    {
        var asOf = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var products = new List<Product>
        {
            new() { Barcode = "A", Title = "Eski", CurrentStock = 10, LastSoldAt = asOf.AddDays(-120) },
            new() { Barcode = "B", Title = "Taze", CurrentStock = 5,  LastSoldAt = asOf.AddDays(-5) },
            new() { Barcode = "C", Title = "Stoksuz", CurrentStock = 0, LastSoldAt = asOf.AddDays(-300) },
        };

        var dead = new DeadStockService().Find(products, asOf, thresholdDays: 90, _ => 40m);

        var item = Assert.Single(dead);
        Assert.Equal("A", item.Barcode);
        Assert.Equal(400m, item.TiedCapital);   // 10 × 40
    }
}

public class CogsCsvParserTests
{
    [Fact]
    public void Gecerli_csv_satirlari_ayristirir()
    {
        var lines = new[]
        {
            "barcode,purchasePrice,vatRate,packagingCost,otherCost,effectiveFrom",
            "BRK1,40.5,20,2,1,2026-01-01",
            "BRK2,30,10,1,0,2026-02-15",
        };

        var result = new CogsCsvParser().Parse(lines);

        Assert.False(result.HasErrors);
        Assert.Equal(2, result.ValidRows.Count);
        Assert.Equal(40.5m, result.ValidRows[0].PurchasePrice);
        Assert.Equal(new DateOnly(2026, 2, 15), result.ValidRows[1].EffectiveFrom);
    }

    [Fact]
    public void Bozuk_satiri_hata_olarak_raporlar()
    {
        var lines = new[]
        {
            "barcode,purchasePrice,vatRate,packagingCost,otherCost,effectiveFrom",
            "BRK1,abc,20,2,1,2026-01-01",
        };

        var result = new CogsCsvParser().Parse(lines);

        Assert.True(result.HasErrors);
        Assert.Empty(result.ValidRows);
    }
}
