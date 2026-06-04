using LeatherErp.Application.Common;
using LeatherErp.Application.Dtos;
using LeatherErp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Services;

/// <summary>
/// Bir ürünün reçetesinden ve eldeki deri stoğunun ağırlıklı ortalama maliyetinden birim maliyet
/// ve önerilen satış fiyatını hesaplar. Hem fiyatlandırma hem raporlama tarafından paylaşılır.
/// </summary>
public class ProductCostingService
{
    private readonly IAppDbContext _db;
    private readonly CostCalculationService _cost;
    private readonly PricingService _pricing;

    public ProductCostingService(IAppDbContext db, CostCalculationService cost, PricingService pricing)
    {
        _db = db;
        _cost = cost;
        _pricing = pricing;
    }

    /// <summary>Ürün için maliyet + fiyat kırılımını hesaplar. Reçete yoksa <see cref="NotFoundException"/>.</summary>
    public async Task<ProductCosting> ComputeAsync(Guid productId, CancellationToken ct = default)
    {
        var recipe = await _db.Recipes.AsNoTracking().FirstOrDefaultAsync(r => r.ProductId == productId, ct)
            ?? throw new NotFoundException("Bu ürün için reçete bulunamadı.");
        var settings = await _db.Settings.AsNoTracking().FirstOrDefaultAsync(ct)
            ?? new Domain.Entities.AppSettings();

        var lots = await _db.LeatherLots.AsNoTracking()
            .Where(l => l.LeatherTypeId == recipe.LeatherTypeId && l.RemainingDm2 > 0).ToListAsync(ct);
        var remaining = lots.Sum(l => l.RemainingDm2);
        var avgUnitCost = remaining > 0 ? lots.Sum(l => l.RemainingDm2 * l.UnitCostPerDm2) / remaining : 0m;

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

        return new ProductCosting
        {
            ProductId = productId,
            LeatherAvgUnitCostPerDm2 = avgUnitCost,
            AvailableLeatherDm2 = remaining,
            Cost = cost,
            Price = price,
            AppliedProfitMargin = settings.DefaultProfitMargin,
            AppliedVatRate = settings.VatRate
        };
    }
}
