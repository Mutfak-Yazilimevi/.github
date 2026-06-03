using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Fiyat analitiği — enflasyona göre reel fiyat erimesi (F4).</summary>
[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly RealPriceService _service;

    public PricingController(AppDbContext db, RealPriceService service)
    {
        _db = db;
        _service = service;
    }

    /// <summary>Ürünün nominal vs reel (TÜFE) fiyat serisi ve erime yüzdesi.</summary>
    [HttpGet("real")]
    public async Task<ActionResult<RealPriceResult>> Real(
        [FromQuery] Guid productId, CancellationToken ct = default)
    {
        var snapshots = await _db.PriceSnapshots
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.CapturedOn)
            .ToListAsync(ct);

        if (snapshots.Count == 0) return NotFound("Bu ürün için fiyat geçmişi yok.");

        var cpi = await _db.InflationIndices.ToDictionaryAsync(x => x.YearMonth, x => x.CpiValue, ct);

        var result = _service.Compute(snapshots, cpi);
        return result is null
            ? NotFound("Hesaplama için yeterli TÜFE verisi yok.")
            : Ok(result);
    }
}
