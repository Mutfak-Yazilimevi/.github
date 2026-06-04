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

        return new ReportSummary
        {
            TotalLeatherStockDm2 = levels.Sum(l => l.RemainingDm2),
            LeatherStockValue = levels.Sum(l => l.StockValue),
            LowStockCount = levels.Count(l => l.IsLow),
            FinishedGoodsValue = finished.Sum(f => f.QuantityOnHand * f.AverageUnitCost),
            FinishedGoodsUnits = finished.Sum(f => f.QuantityOnHand),
            ProductionOrderCount = orders.Count,
            UnitsProduced = orders.Where(o => o.Status == ProductionStatus.Confirmed).Sum(o => o.Quantity)
        };
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
