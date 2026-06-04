using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Catalog;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Ingestion;

/// <summary>
/// Muhasebe sağlayıcısından alış maliyetlerini çekip ProductCost'a effective-dated olarak yazar
/// (COGS üçüncü kaynağı: CostSource.AccountingIntegration).
/// </summary>
public class AccountingCostImportService
{
    private readonly AppDbContext _db;
    private readonly IAccountingProvider _provider;
    private readonly ILogger<AccountingCostImportService> _logger;

    public AccountingCostImportService(
        AppDbContext db, IAccountingProvider provider, ILogger<AccountingCostImportService> logger)
    {
        _db = db;
        _provider = provider;
        _logger = logger;
    }

    public async Task ImportForTenantAsync(Guid tenantId, DateOnly since, CancellationToken ct = default)
    {
        var records = await _provider.GetPurchaseCostsAsync(since, ct);
        if (records.Count == 0) return;

        var barcodes = records.Select(r => r.Barcode).ToHashSet();
        var products = await _db.Products.IgnoreQueryFilters()
            .Where(p => p.TenantId == tenantId && barcodes.Contains(p.Barcode))
            .ToDictionaryAsync(p => p.Barcode, ct);

        var applied = 0;
        foreach (var r in records)
        {
            if (!products.TryGetValue(r.Barcode, out var product)) continue;

            // Önceki açık maliyeti kapat (effective-dating).
            var previousOpen = await _db.ProductCosts.IgnoreQueryFilters()
                .Where(c => c.ProductId == product.Id && c.EffectiveTo == null && c.EffectiveFrom < r.EffectiveFrom)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);
            if (previousOpen is not null)
                previousOpen.EffectiveTo = r.EffectiveFrom.AddDays(-1);

            _db.ProductCosts.Add(new ProductCost
            {
                TenantId = tenantId,
                ProductId = product.Id,
                EffectiveFrom = r.EffectiveFrom,
                PurchasePrice = r.PurchasePrice,
                VatRate = r.VatRate,
                Source = CostSource.AccountingIntegration
            });
            applied++;
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Muhasebeden {Applied}/{Total} maliyet içe aktarıldı ({Provider})",
            applied, records.Count, _provider.Name);
    }
}
