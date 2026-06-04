using LeatherErp.Api.Models;
using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly CostCalculationService _cost;
    private readonly PricingService _pricing;
    private readonly ProductCostingService _costing;

    public PricingController(CostCalculationService cost, PricingService pricing, ProductCostingService costing)
    {
        _cost = cost;
        _pricing = pricing;
        _costing = costing;
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
    public async Task<ActionResult<ProductCosting>> CalculateForProduct(Guid productId)
        => Ok(await _costing.ComputeAsync(productId));
}
