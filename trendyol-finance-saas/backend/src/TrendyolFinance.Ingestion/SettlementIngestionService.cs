using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Trendyol;

namespace TrendyolFinance.Ingestion;

/// <summary>
/// Bir mağaza için hakediş verisini çeker ve idempotent biçimde saklar.
/// İlk yükleme (backfill) ve artımlı (incremental) senkron destekler.
/// </summary>
public class SettlementIngestionService
{
    private readonly AppDbContext _db;
    private readonly ITrendyolApiClient _client;
    private readonly ITrendyolCredentialResolver _credentials;
    private readonly ILogger<SettlementIngestionService> _logger;

    public SettlementIngestionService(
        AppDbContext db,
        ITrendyolApiClient client,
        ITrendyolCredentialResolver credentials,
        ILogger<SettlementIngestionService> logger)
    {
        _db = db;
        _client = client;
        _credentials = credentials;
        _logger = logger;
    }

    /// <summary>İlk tarihsel yükleme: son <paramref name="months"/> ayı çeker.</summary>
    public async Task BackfillAsync(Guid sellerAccountId, int months = 12, CancellationToken ct = default)
    {
        var account = await _db.SellerAccounts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == sellerAccountId, ct)
            ?? throw new InvalidOperationException($"SellerAccount {sellerAccountId} bulunamadı.");

        var end = DateTimeOffset.UtcNow;
        var start = end.AddMonths(-months);
        var count = await IngestWindowAsync(account.TenantId, account.Id, _credentials.Resolve(account), start, end, ct);

        account.BackfillCompleted = true;
        account.LastSettlementSyncedAt = end;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Backfill tamam: {Account} — {Count} kalem", sellerAccountId, count);
    }

    /// <summary>Artımlı senkron: son başarılı çekimden bugüne.</summary>
    public async Task SyncIncrementalAsync(Guid sellerAccountId, CancellationToken ct = default)
    {
        var account = await _db.SellerAccounts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == sellerAccountId, ct)
            ?? throw new InvalidOperationException($"SellerAccount {sellerAccountId} bulunamadı.");

        var end = DateTimeOffset.UtcNow;
        // Geç gelen düzeltmeleri yakalamak için son senkrondan 2 gün geriye taşarak çek.
        var start = (account.LastSettlementSyncedAt ?? end.AddDays(-2)).AddDays(-2);
        var count = await IngestWindowAsync(account.TenantId, account.Id, _credentials.Resolve(account), start, end, ct);

        account.LastSettlementSyncedAt = end;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Artımlı senkron tamam: {Account} — {Count} kalem", sellerAccountId, count);
    }

    private async Task<int> IngestWindowAsync(
        Guid tenantId, Guid sellerAccountId, TrendyolCredentials creds,
        DateTimeOffset start, DateTimeOffset end, CancellationToken ct)
    {
        // Bu pencerede zaten kayıtlı kalem id'leri (idempotent upsert için).
        var existingIds = await _db.SettlementTransactions.IgnoreQueryFilters()
            .Where(t => t.SellerAccountId == sellerAccountId
                && t.TransactionDate >= start && t.TransactionDate <= end)
            .Select(t => t.TrendyolTransactionId)
            .ToHashSetAsync(ct);

        var buffer = new List<Domain.Finance.SettlementTransaction>(512);
        var added = 0;

        await foreach (var dto in _client.StreamSettlementsAsync(creds, start, end, ct))
        {
            if (existingIds.Contains(dto.Id)) continue;
            existingIds.Add(dto.Id);
            buffer.Add(SettlementMapper.ToEntity(dto, tenantId, sellerAccountId));
            added++;

            if (buffer.Count >= 500)
            {
                _db.SettlementTransactions.AddRange(buffer);
                await _db.SaveChangesAsync(ct);
                buffer.Clear();
            }
        }

        if (buffer.Count > 0)
        {
            _db.SettlementTransactions.AddRange(buffer);
            await _db.SaveChangesAsync(ct);
        }

        return added;
    }
}
