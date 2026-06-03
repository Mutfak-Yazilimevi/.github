using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Ingestion;

/// <summary>Hangfire tarafından çağrılan ingestion görevleri (recurring + fire-and-forget).</summary>
public class IngestionJobs
{
    private readonly SettlementIngestionService _settlements;
    private readonly AppDbContext _db;

    public IngestionJobs(SettlementIngestionService settlements, AppDbContext db)
    {
        _settlements = settlements;
        _db = db;
    }

    /// <summary>Yeni bağlanan mağaza için ilk yükleme (fire-and-forget enqueue edilir).</summary>
    public Task BackfillAccountAsync(Guid sellerAccountId, CancellationToken ct = default) =>
        _settlements.BackfillAsync(sellerAccountId, months: 12, ct: ct);

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
}
