using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Trendyol;

namespace TrendyolFinance.Ingestion;

/// <summary>
/// Trendyol kategori komisyon oranlarını çekip CategoryCommission tablosuna upsert eder (F3 referans verisi).
/// Komisyon oranları tenant'tan bağımsızdır; herhangi bir aktif mağaza kimliğiyle çekilebilir.
/// </summary>
public class CategoryCommissionSyncService
{
    private readonly AppDbContext _db;
    private readonly ITrendyolCatalogClient _catalog;
    private readonly ITrendyolCredentialResolver _credentials;
    private readonly ILogger<CategoryCommissionSyncService> _logger;

    public CategoryCommissionSyncService(
        AppDbContext db, ITrendyolCatalogClient catalog,
        ITrendyolCredentialResolver credentials, ILogger<CategoryCommissionSyncService> logger)
    {
        _db = db;
        _catalog = catalog;
        _credentials = credentials;
        _logger = logger;
    }

    public async Task SyncAsync(CancellationToken ct = default)
    {
        var account = await _db.SellerAccounts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.IsActive, ct);
        if (account is null)
        {
            _logger.LogWarning("Aktif mağaza yok; kategori komisyon senkronu atlandı.");
            return;
        }

        var commissions = await _catalog.GetCategoryCommissionsAsync(_credentials.Resolve(account), ct);
        if (commissions.Count == 0) return;

        var existing = await _db.CategoryCommissions.ToDictionaryAsync(x => x.TrendyolCategoryId, ct);

        foreach (var c in commissions)
        {
            if (existing.TryGetValue(c.CategoryId, out var row))
            {
                row.CommissionRate = c.CommissionRate;
                row.CategoryName = c.CategoryName;
                row.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                _db.CategoryCommissions.Add(new CategoryCommission
                {
                    TrendyolCategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CommissionRate = c.CommissionRate
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Kategori komisyonları senkronlandı: {Count}", commissions.Count);
    }
}
