using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Hakediş mutabakatı (F3): Trendyol'un kestiği komisyonu bağımsız doğrular.</summary>
[ApiController]
[Route("api/[controller]")]
public class ReconciliationController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ReconciliationService _service;

    public ReconciliationController(AppDbContext db, ReconciliationService service)
    {
        _db = db;
        _service = service;
    }

    /// <summary>Dönem için komisyon sapmalarını listeler. Örn: /api/reconciliation?from=...&to=...</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReconciliationFinding>>> Get(
        [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to, CancellationToken ct)
    {
        var sales = await _db.SettlementTransactions
            .Where(t => t.TransactionType == SettlementTransactionType.Sale
                && t.TransactionDate >= from && t.TransactionDate <= to)
            .ToListAsync(ct);

        // Beklenen komisyon oranını ürünün kategorisinden çöz.
        var rateByBarcode = await _db.Products
            .Where(p => p.TrendyolCategoryId != null)
            .ToDictionaryAsync(p => p.Barcode, p => p.TrendyolCategoryId, ct);

        var categoryRates = await _db.Set<CategoryCommission>()
            .ToDictionaryAsync(c => c.TrendyolCategoryId, c => c.CommissionRate, ct);

        decimal? RateResolver(string? barcode)
        {
            if (barcode is null || !rateByBarcode.TryGetValue(barcode, out var catId) || catId is null) return null;
            return categoryRates.TryGetValue(catId.Value, out var rate) ? rate : null;
        }

        var findings = _service.Check(sales, RateResolver).ToList();
        return Ok(findings);
    }
}
