using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Kârlılık raporları (F1/F2). Tenant filtresi DbContext'te global olarak uygulanır.</summary>
[ApiController]
[Route("api/[controller]")]
public class ProfitController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ProfitCalculator _calculator;

    public ProfitController(AppDbContext db, ProfitCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    /// <summary>Bir dönem için özet kârlılık. Örn: /api/profit/summary?from=2026-01-01&to=2026-01-31</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ProfitResult>> GetSummary(
        [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to, CancellationToken ct)
    {
        var transactions = await _db.SettlementTransactions
            .Where(t => t.TransactionDate >= from && t.TransactionDate <= to)
            .ToListAsync(ct);

        // Satış anındaki etkin maliyeti çözen yardımcı (effective-dated COGS).
        var costs = await _db.ProductCosts.ToListAsync(ct);
        var products = await _db.Products.ToDictionaryAsync(p => p.Barcode, p => p.Id, ct);

        decimal? CostResolver(string? barcode, DateTimeOffset onDate)
        {
            if (barcode is null || !products.TryGetValue(barcode, out var productId)) return null;
            var d = DateOnly.FromDateTime(onDate.UtcDateTime);
            var cost = costs.FirstOrDefault(c => c.ProductId == productId
                && c.EffectiveFrom <= d && (c.EffectiveTo == null || c.EffectiveTo >= d));
            return cost?.TotalUnitCost;
        }

        var result = _calculator.Calculate(transactions, CostResolver);
        return Ok(result);
    }
}
