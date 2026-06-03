using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Öngörü (F6): geçmiş satıştan gelecek dönem tahmini.</summary>
[ApiController]
[Route("api/[controller]")]
public class ForecastController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ForecastService _service;

    public ForecastController(AppDbContext db, ForecastService service)
    {
        _db = db;
        _service = service;
    }

    /// <summary>Aylık satış cirosu tahmini. Opsiyonel barkod ile tek ürün. /api/forecast/sales?horizon=3</summary>
    [HttpGet("sales")]
    public async Task<ActionResult<ForecastResult>> Sales(
        [FromQuery] string? barcode, [FromQuery] int horizon = 3, CancellationToken ct = default)
    {
        var query = _db.SettlementTransactions
            .Where(t => t.TransactionType == SettlementTransactionType.Sale);

        if (!string.IsNullOrWhiteSpace(barcode))
            query = query.Where(t => t.Barcode == barcode);

        // Aylık brüt ciro (YYYYMM bazında).
        var monthly = await query
            .GroupBy(t => t.TransactionDate.Year * 100 + t.TransactionDate.Month)
            .Select(g => new SeriesPoint(g.Key, g.Sum(x => x.GrossAmount)))
            .ToListAsync(ct);

        return Ok(_service.Forecast(monthly, horizon));
    }
}
