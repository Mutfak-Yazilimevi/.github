using LeatherErp.Api.Models;
using LeatherErp.Application.Common;
using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CostCalculationService _cost;
    private readonly PricingService _pricing;

    public PricingController(AppDbContext db, CostCalculationService cost, PricingService pricing)
    {
        _db = db;
        _cost = cost;
        _pricing = pricing;
    }

    /// <summary>Serbest girdiyle birim maliyet kırılımı hesaplar.</summary>
    [HttpPost("cost")]
    public ActionResult<CostBreakdown> CalculateCost(CostCalcRequest req)
        => Ok(_cost.Calculate(new CostInput
        {
            NetLeatherDm2 = req.NetLeatherDm2,
            WasteRate = req.WasteRate,
            UnitCostPerDm2 = req.UnitCostPerDm2,
            LaborCost = req.LaborCost,
            OverheadCost = req.OverheadCost
        }));

    /// <summary>Maliyetten KDV dahil satış fiyatı hesaplar.</summary>
    [HttpPost("price")]
    public ActionResult<PricingResult> CalculatePrice(PriceCalcRequest req)
        => Ok(_pricing.Calculate(new PricingInput
        {
            UnitCost = req.UnitCost,
            ProfitMargin = req.ProfitMargin,
            VatRate = req.VatRate
        }));

    /// <summary>Hedef satış fiyatından gerçekleşen kâr marjını çözer.</summary>
    [HttpPost("reverse")]
    public ActionResult<object> ReversePrice(ReversePriceRequest req)
    {
        var margin = _pricing.SolveProfitMargin(new ReversePricingInput
        {
            UnitCost = req.UnitCost,
            TargetSalePrice = req.TargetSalePrice,
            VatRate = req.VatRate
        });
        return Ok(new { profitMargin = margin });
    }

    /// <summary>
    /// Bir ürünün reçetesinden ve eldeki deri stoğunun ağırlıklı ortalama maliyetinden
    /// birim maliyet ve önerilen satış fiyatını hesaplar.
    /// </summary>
    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<object>> CalculateForProduct(Guid productId)
    {
        var recipe = await _db.Recipes.AsNoTracking().FirstOrDefaultAsync(r => r.ProductId == productId)
            ?? throw new NotFoundException("Bu ürün için reçete bulunamadı.");
        var settings = await _db.Settings.AsNoTracking().FirstOrDefaultAsync() ?? new Domain.Entities.AppSettings();

        var lots = await _db.LeatherLots.AsNoTracking()
            .Where(l => l.LeatherTypeId == recipe.LeatherTypeId && l.RemainingDm2 > 0).ToListAsync();
        var remaining = lots.Sum(l => l.RemainingDm2);
        var avgUnitCost = remaining > 0
            ? lots.Sum(l => l.RemainingDm2 * l.UnitCostPerDm2) / remaining
            : 0m;

        var cost = _cost.Calculate(new CostInput
        {
            NetLeatherDm2 = recipe.NetLeatherDm2,
            WasteRate = recipe.WasteRate,
            UnitCostPerDm2 = avgUnitCost,
            LaborCost = recipe.LaborCost,
            OverheadCost = recipe.OverheadCost
        });

        var price = _pricing.Calculate(new PricingInput
        {
            UnitCost = cost.UnitCost,
            ProfitMargin = settings.DefaultProfitMargin,
            VatRate = settings.VatRate
        });

        return Ok(new
        {
            productId,
            leatherAvgUnitCostPerDm2 = avgUnitCost,
            availableLeatherDm2 = remaining,
            cost,
            price,
            appliedProfitMargin = settings.DefaultProfitMargin,
            appliedVatRate = settings.VatRate
        });
    }
}
