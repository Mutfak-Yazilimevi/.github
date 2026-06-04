using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Ingestion;

/// <summary>Hangfire tarafından çağrılan ingestion görevleri (recurring + fire-and-forget).</summary>
public class IngestionJobs
{
    private readonly SettlementIngestionService _settlements;
    private readonly InflationImportService _inflation;
    private readonly CategoryCommissionSyncService _commissions;
    private readonly ProductSyncService _products;
    private readonly AppDbContext _db;

    public IngestionJobs(
        SettlementIngestionService settlements,
        InflationImportService inflation,
        CategoryCommissionSyncService commissions,
        ProductSyncService products,
        AppDbContext db)
    {
        _settlements = settlements;
        _inflation = inflation;
        _commissions = commissions;
        _products = products;
        _db = db;
    }

    /// <summary>Yeni bağlanan mağaza için ilk yükleme: settlement backfill + ürün senkronu.</summary>
    public async Task BackfillAccountAsync(Guid sellerAccountId, CancellationToken ct = default)
    {
        await _settlements.BackfillAsync(sellerAccountId, months: 12, ct: ct);
        await _products.SyncAsync(sellerAccountId, ct);
    }

    /// <summary>Tüm aktif mağazalar için saatlik artımlı senkron (recurring job).</summary>
    public async Task SyncAllActiveAsync(CancellationToken ct = default)
    {
        var accountIds = await _db.SellerAccounts.IgnoreQueryFilters()
            .Where(a => a.IsActive && a.BackfillCompleted)
            .Select(a => a.Id)
            .ToListAsync(ct);

        foreach (var id in accountIds)
            await _settlements.SyncIncrementalAsync(id, ct);
    }

    /// <summary>Tüm aktif mağazalar için günlük ürün/fiyat senkronu (recurring).</summary>
    public async Task SyncAllProductsAsync(CancellationToken ct = default)
    {
        var accountIds = await _db.SellerAccounts.IgnoreQueryFilters()
            .Where(a => a.IsActive && a.BackfillCompleted)
            .Select(a => a.Id)
            .ToListAsync(ct);

        foreach (var id in accountIds)
            await _products.SyncAsync(id, ct);
    }

    /// <summary>Aylık TÜFE içe aktarımı (recurring).</summary>
    public Task ImportInflationAsync(CancellationToken ct = default) =>
        _inflation.ImportAsync(ct: ct);

    /// <summary>Günlük kategori komisyon senkronu (recurring).</summary>
    public Task SyncCategoryCommissionsAsync(CancellationToken ct = default) =>
        _commissions.SyncAsync(ct);
}
