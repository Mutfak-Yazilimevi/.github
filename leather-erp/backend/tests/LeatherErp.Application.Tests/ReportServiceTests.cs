using LeatherErp.Application.Services;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeatherErp.Application.Tests;

public class ReportServiceTests
{
    private static AppDbContext NewDb()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static ReportService BuildService(AppDbContext db)
    {
        var cost = new CostCalculationService();
        var pricing = new PricingService();
        return new ReportService(db, new StockService(db), new ProductCostingService(db, cost, pricing));
    }

    private static AppDbContext Seed()
    {
        var db = NewDb();
        var type = new LeatherType { Name = "Vaketa", LowStockThresholdDm2 = 100m };
        var product = new Product { Name = "Cüzdan", IsActive = true };
        var recipe = new ProductRecipe
        {
            Product = product, LeatherType = type,
            NetLeatherDm2 = 8m, WasteRate = 0.15m, LaborCost = 40m, OverheadCost = 15m
        };
        var lot = new LeatherLot
        {
            LeatherType = type, QuantityDm2 = 500m, RemainingDm2 = 500m, UnitCostPerDm2 = 12.50m
        };
        db.AddRange(new AppSettings { DefaultProfitMargin = 0.40m, VatRate = 0.20m }, type, product, recipe, lot);
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetSummary_AggregatesStockValue()
    {
        var db = Seed();
        var summary = await BuildService(db).GetSummaryAsync();

        Assert.Equal(500m, summary.TotalLeatherStockDm2);
        Assert.Equal(6250m, summary.LeatherStockValue);   // 500 × 12.50
        Assert.Equal(0, summary.LowStockCount);            // 500 > 100 eşik
        Assert.Equal(0, summary.UnitsProduced);
    }

    [Fact]
    public async Task GetProfitability_ComputesCostPriceAndProducibleUnits()
    {
        var db = Seed();
        var rows = await BuildService(db).GetProfitabilityAsync();

        var row = Assert.Single(rows);
        // 8 net × 12.50 × 1.15 = 115 malzeme + 55 = 170 maliyet.
        Assert.Equal(170m, row.UnitCost);
        // 170 × 1.40 × 1.20 = 285.60 satış.
        Assert.Equal(285.60m, row.SalePrice);
        Assert.Equal(0.40m, row.ProfitMargin);
        // Brüt birim 9.2 dm² → 500 / 9.2 = 54 adet üretilebilir.
        Assert.Equal(54, row.ProducibleUnits);
    }

    [Fact]
    public async Task GetProductionTrend_BucketsByMonth_AndZeroFills()
    {
        var db = Seed();
        var productId = db.Products.First().Id;
        var now = DateTime.UtcNow;
        // Bu ay 2 onaylı emir, geçen ay 1; aradaki/diğer aylar boş kalmalı.
        db.ProductionOrders.AddRange(
            new ProductionOrder { ProductId = productId, Quantity = 3, Status = Domain.Enums.ProductionStatus.Confirmed, UnitCostSnapshot = 100m, OrderDate = now },
            new ProductionOrder { ProductId = productId, Quantity = 2, Status = Domain.Enums.ProductionStatus.Confirmed, UnitCostSnapshot = 100m, OrderDate = now },
            new ProductionOrder { ProductId = productId, Quantity = 4, Status = Domain.Enums.ProductionStatus.Confirmed, UnitCostSnapshot = 50m, OrderDate = now.AddMonths(-1) },
            // Taslak emir sayılmamalı.
            new ProductionOrder { ProductId = productId, Quantity = 9, Status = Domain.Enums.ProductionStatus.Draft, OrderDate = now });
        // Bu ay 2 adet satış (gelir serisi için).
        db.SalesOrders.Add(new SalesOrder { ProductId = productId, Quantity = 2, UnitPrice = 290m, UnitCost = 170m, SaleDate = now });
        db.SaveChanges();

        var trend = await BuildService(db).GetProductionTrendAsync(6);

        Assert.Equal(6, trend.Count);                       // 6 ay, sürekli
        Assert.Equal(5, trend[^1].Units);                   // bu ay: 3 + 2
        Assert.Equal(500m, trend[^1].Value);                // 5 × 100
        Assert.Equal(2, trend[^1].SalesUnits);              // bu ay satış adedi
        Assert.Equal(580m, trend[^1].Revenue);              // 2 × 290
        Assert.Equal(4, trend[^2].Units);                   // geçen ay
        Assert.Equal(200m, trend[^2].Value);                // 4 × 50
        Assert.All(trend.Take(4), p => Assert.Equal(0, p.Units));   // önceki aylar sıfır
    }

    [Fact]
    public async Task GetProfitability_IgnoresProductsWithoutRecipe()
    {
        var db = Seed();
        db.Products.Add(new Product { Name = "Reçetesiz", IsActive = true });
        db.SaveChanges();

        var rows = await BuildService(db).GetProfitabilityAsync();
        Assert.Single(rows);   // yalnızca reçetesi olan ürün
    }
}
