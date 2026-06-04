using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

public record DashboardResponse(
    ProfitResult CurrentPeriod,
    ProfitResult PreviousPeriod,
    decimal RevenueGrowthPercent,
    HealthScore Health);

/// <summary>İşletme sağlık panosu (F2): "İşletmem iyi mi kötü mü?" özeti.</summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ProfitCalculator _profit;
    private readonly HealthScoreCalculator _health;

    public DashboardController(AppDbContext db, ProfitCalculator profit, HealthScoreCalculator health)
    {
        _db = db;
        _profit = profit;
        _health = health;
    }

    /// <summary>Verilen ay için özet + bir önceki ayla karşılaştırma + sağlık skoru. /api/dashboard?month=2026-06</summary>
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get([FromQuery] string? month, CancellationToken ct)
    {
        var (curFrom, curTo) = MonthRange(month);
        var prevFrom = curFrom.AddMonths(-1);

        var current = await CalcPeriodAsync(curFrom, curTo, ct);
        var previous = await CalcPeriodAsync(prevFrom, curFrom, ct);

        var growth = previous.GrossRevenue == 0 ? 0
            : Math.Round((current.GrossRevenue - previous.GrossRevenue) / previous.GrossRevenue * 100, 2);

        var marginTrend = current.MarginPercent - previous.MarginPercent;
        var returnRate = current.GrossRevenue == 0 ? 0
            : Math.Round(current.Deductions / current.GrossRevenue * 100, 2);

        // Stok devir hızı (yıllık): dönemdeki satış adedi / mevcut toplam stok, yıla ölçeklenmiş.
        var soldUnits = await _db.SettlementTransactions
            .CountAsync(t => t.TransactionType == SettlementTransactionType.Sale
                && t.TransactionDate >= curFrom && t.TransactionDate < curTo, ct);
        var totalStock = await _db.Products.SumAsync(p => p.CurrentStock, ct);
        var periodDays = Math.Max((curTo - curFrom).TotalDays, 1);
        var turnover = totalStock <= 0 ? 0
            : Math.Round((decimal)(soldUnits / (double)totalStock * (365.0 / periodDays)), 2);

        var health = _health.Calculate(new HealthInputs(
            MarginPercent: current.MarginPercent,
            MarginTrendPercent: marginTrend,
            ReturnRatePercent: returnRate,
            RevenueGrowthPercent: growth,
            StockTurnover: turnover));

        return Ok(new DashboardResponse(current, previous, growth, health));
    }

    private async Task<ProfitResult> CalcPeriodAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        var txns = await _db.SettlementTransactions
            .Where(t => t.TransactionDate >= from && t.TransactionDate < to)
            .ToListAsync(ct);

        var products = await _db.Products.ToDictionaryAsync(p => p.Barcode, p => p.Id, ct);
        var costs = await _db.ProductCosts.ToListAsync(ct);

        decimal? CostResolver(string? barcode, DateTimeOffset onDate)
        {
            if (barcode is null || !products.TryGetValue(barcode, out var pid)) return null;
            var d = DateOnly.FromDateTime(onDate.UtcDateTime);
            return costs.FirstOrDefault(c => c.ProductId == pid
                && c.EffectiveFrom <= d && (c.EffectiveTo == null || c.EffectiveTo >= d))?.TotalUnitCost;
        }

        return _profit.Calculate(txns, CostResolver);
    }

    private static (DateTimeOffset From, DateTimeOffset To) MonthRange(string? month)
    {
        var now = DateTimeOffset.UtcNow;
        var first = DateTime.TryParse(month is null ? null : month + "-01", out var d)
            ? new DateTimeOffset(d, TimeSpan.Zero)
            : new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        return (first, first.AddMonths(1));
    }
}
