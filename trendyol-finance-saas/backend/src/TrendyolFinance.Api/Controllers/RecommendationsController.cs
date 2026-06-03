using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Pazarlama/fiyat önerileri (F7): analitik çıktılarını kural setiyle önerilere çevirir.</summary>
[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly DeadStockService _deadStock;
    private readonly ProfitCalculator _profit;
    private readonly ReconciliationService _reconciliation;
    private readonly RecommendationService _recommendations;

    public RecommendationsController(
        AppDbContext db, DeadStockService deadStock, ProfitCalculator profit,
        ReconciliationService reconciliation, RecommendationService recommendations)
    {
        _db = db;
        _deadStock = deadStock;
        _profit = profit;
        _reconciliation = reconciliation;
        _recommendations = recommendations;
    }

    /// <summary>Son 90 gün verisine göre öneriler.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recommendation>>> Get(CancellationToken ct)
    {
        var to = DateTimeOffset.UtcNow;
        var from = to.AddDays(-90);

        // Ölü stok
        var products = await _db.Products.Where(p => p.CurrentStock > 0).ToListAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var openCosts = await _db.ProductCosts
            .Where(c => c.EffectiveTo == null || c.EffectiveTo >= today).ToListAsync(ct);
        decimal? CostResolver(Guid pid) => openCosts.Where(c => c.ProductId == pid)
            .OrderByDescending(c => c.EffectiveFrom).FirstOrDefault()?.TotalUnitCost;
        var deadStock = _deadStock.Find(products, to, 90, CostResolver);

        // Genel marj
        var txns = await _db.SettlementTransactions
            .Where(t => t.TransactionDate >= from && t.TransactionDate <= to).ToListAsync(ct);
        var productIdByBarcode = products.ToDictionary(p => p.Barcode, p => p.Id);
        var allCosts = await _db.ProductCosts.ToListAsync(ct);
        decimal? CostByBarcode(string? barcode, DateTimeOffset onDate)
        {
            if (barcode is null || !productIdByBarcode.TryGetValue(barcode, out var pid)) return null;
            var d = DateOnly.FromDateTime(onDate.UtcDateTime);
            return allCosts.FirstOrDefault(c => c.ProductId == pid
                && c.EffectiveFrom <= d && (c.EffectiveTo == null || c.EffectiveTo >= d))?.TotalUnitCost;
        }
        var profit = _profit.Calculate(txns, CostByBarcode);

        // Kritik hakediş sapması sayısı
        var sales = txns.Where(t => t.TransactionType == SettlementTransactionType.Sale).ToList();
        var categoryRates = await _db.CategoryCommissions
            .ToDictionaryAsync(c => c.TrendyolCategoryId, c => c.CommissionRate, ct);
        var catByBarcode = products.Where(p => p.TrendyolCategoryId != null)
            .ToDictionary(p => p.Barcode, p => p.TrendyolCategoryId!.Value);
        decimal? RateResolver(string? barcode) =>
            barcode != null && catByBarcode.TryGetValue(barcode, out var cat)
            && categoryRates.TryGetValue(cat, out var rate) ? rate : null;
        var criticalCount = _reconciliation.Check(sales, RateResolver)
            .Count(f => f.Severity == ReconciliationSeverity.Critical);

        // Fiyat erimesi girdisi şimdilik boş (PriceSnapshot + TÜFE dolunca beslenecek).
        var priceErosions = Array.Empty<(string, decimal)>();

        var recs = _recommendations.Build(priceErosions, deadStock, profit.MarginPercent, criticalCount);
        return Ok(recs);
    }
}
