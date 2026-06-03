using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

/// <summary>Stok analitiği — ölü stok / yavaş satan (F5).</summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly DeadStockService _service;

    public InventoryController(AppDbContext db, DeadStockService service)
    {
        _db = db;
        _service = service;
    }

    /// <summary>N gündür satılmayan, stoğu olan ürünler + bağlı sermaye. Örn: /api/inventory/dead-stock?days=90</summary>
    [HttpGet("dead-stock")]
    public async Task<ActionResult<IEnumerable<DeadStockItem>>> DeadStock(
        [FromQuery] int days = 90, CancellationToken ct = default)
    {
        var products = await _db.Products.Where(p => p.CurrentStock > 0).ToListAsync(ct);

        // Ürünün güncel (açık) birim maliyeti.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var openCosts = await _db.ProductCosts
            .Where(c => c.EffectiveTo == null || c.EffectiveTo >= today)
            .ToListAsync(ct);

        decimal? CostResolver(Guid productId) =>
            openCosts.Where(c => c.ProductId == productId)
                     .OrderByDescending(c => c.EffectiveFrom)
                     .FirstOrDefault()?.TotalUnitCost;

        var result = _service.Find(products, DateTimeOffset.UtcNow, days, CostResolver);
        return Ok(result);
    }
}
