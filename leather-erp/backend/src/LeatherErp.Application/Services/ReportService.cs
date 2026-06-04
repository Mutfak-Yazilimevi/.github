using LeatherErp.Application.Common;
using LeatherErp.Application.Dtos;
using LeatherErp.Application.Interfaces;
using LeatherErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Services;

/// <summary>Panel/rapor verilerini üretir: genel özet ve ürün bazında kârlılık.</summary>
public class ReportService
{
    private readonly IAppDbContext _db;
    private readonly StockService _stock;
    private readonly ProductCostingService _costing;

    public ReportService(IAppDbContext db, StockService stock, ProductCostingService costing)
    {
        _db = db;
        _stock = stock;
        _costing = costing;
    }

    /// <summary>Stok, mamul ve üretim KPI'larını tek çağrıda döndürür.</summary>
    public async Task<ReportSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        var levels = await _stock.GetStockLevelsAsync(ct);
        var finished = await _db.FinishedGoods.AsNoTracking().ToListAsync(ct);
        var orders = await _db.ProductionOrders.AsNoTracking().ToListAsync(ct);
        var sales = await _db.SalesOrders.AsNoTracking().ToListAsync(ct);

        return new ReportSummary
        {
            TotalLeatherStockDm2 = levels.Sum(l => l.RemainingDm2),
            LeatherStockValue = levels.Sum(l => l.StockValue),
            LowStockCount = levels.Count(l => l.IsLow),
            FinishedGoodsValue = finished.Sum(f => f.QuantityOnHand * f.AverageUnitCost),
            FinishedGoodsUnits = finished.Sum(f => f.QuantityOnHand),
            ProductionOrderCount = orders.Count,
            UnitsProduced = orders.Where(o => o.Status == ProductionStatus.Confirmed).Sum(o => o.Quantity),
            TotalRevenue = sales.Sum(x => x.Quantity * x.UnitPrice),
            TotalProfit = sales.Sum(x => x.Quantity * (x.UnitPrice - x.UnitCost)),
            UnitsSold = sales.Sum(x => x.Quantity)
        };
    }

    private static readonly string[] MonthAbbr =
        { "Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara" };

    /// <summary>
    /// Son <paramref name="months"/> ay için aylık üretim trend'ini döndürür (onaylanan emirler).
    /// Üretim olmayan aylar sıfır değerle doldurulur, böylece grafik sürekli olur.
    /// </summary>
    public async Task<List<ProductionTrendPoint>> GetProductionTrendAsync(int months = 6, CancellationToken ct = default)
    {
        if (months < 1) months = 1;
        if (months > 36) months = 36;

        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(months - 1));

        var orders = await _db.ProductionOrders.AsNoTracking()
            .Where(o => o.Status == ProductionStatus.Confirmed && o.OrderDate >= start)
            .ToListAsync(ct);
        var sales = await _db.SalesOrders.AsNoTracking()
            .Where(x => x.SaleDate >= start)
            .ToListAsync(ct);

        var prodByMonth = orders
            .GroupBy(o => (o.OrderDate.Year, o.OrderDate.Month))
            .ToDictionary(g => g.Key, g => (
                Units: g.Sum(o => o.Quantity),
                Value: g.Sum(o => o.UnitCostSnapshot * o.Quantity)));
        var salesByMonth = sales
            .GroupBy(x => (x.SaleDate.Year, x.SaleDate.Month))
            .ToDictionary(g => g.Key, g => (
                Units: g.Sum(x => x.Quantity),
                Revenue: g.Sum(x => x.Quantity * x.UnitPrice)));

        var points = new List<ProductionTrendPoint>(months);
        for (var i = 0; i < months; i++)
        {
            var d = start.AddMonths(i);
            prodByMonth.TryGetValue((d.Year, d.Month), out var prod);
            salesByMonth.TryGetValue((d.Year, d.Month), out var sale);
            points.Add(new ProductionTrendPoint
            {
                Period = $"{d.Year:0000}-{d.Month:00}",
                Label = $"{MonthAbbr[d.Month - 1]} {d.Year}",
                Units = prod.Units,
                Value = prod.Value,
                SalesUnits = sale.Units,
                Revenue = sale.Revenue
            });
        }
        return points;
    }

    /// <summary>Reçetesi olan aktif ürünler için kârlılık tablosu üretir.</summary>
    public async Task<List<ProductProfitability>> GetProfitabilityAsync(CancellationToken ct = default)
    {
        var products = await _db.Products.AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Recipe)
            .ToListAsync(ct);

        var result = new List<ProductProfitability>();
        foreach (var p in products.Where(p => p.Recipe is not null))
        {
            var c = await _costing.ComputeAsync(p.Id, ct);
            var grossPerUnit = p.Recipe!.NetLeatherDm2 * (1 + p.Recipe.WasteRate);
            var producible = grossPerUnit > 0 ? (int)Math.Floor(c.AvailableLeatherDm2 / grossPerUnit) : 0;

            result.Add(new ProductProfitability
            {
                ProductId = p.Id,
                ProductName = p.Name,
                UnitCost = c.Cost.UnitCost,
                SalePrice = c.Price.SalePrice,
                UnitProfit = c.Price.ProfitAmount,
                ProfitMargin = c.AppliedProfitMargin,
                ProducibleUnits = producible
            });
        }
        return result.OrderByDescending(r => r.UnitProfit).ToList();
    }
}
