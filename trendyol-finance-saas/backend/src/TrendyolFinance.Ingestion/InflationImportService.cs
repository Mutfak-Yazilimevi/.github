using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Inflation;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Ingestion;

/// <summary>TÜFE serisini sağlayıcıdan çekip InflationIndex tablosuna upsert eder (F4 referans verisi).</summary>
public class InflationImportService
{
    private readonly AppDbContext _db;
    private readonly IInflationProvider _provider;
    private readonly ILogger<InflationImportService> _logger;

    public InflationImportService(AppDbContext db, IInflationProvider provider, ILogger<InflationImportService> logger)
    {
        _db = db;
        _provider = provider;
        _logger = logger;
    }

    public async Task ImportAsync(int fromYearMonth = 202001, CancellationToken ct = default)
    {
        var series = await _provider.GetCpiSeriesAsync(fromYearMonth, ct);
        if (series.Count == 0)
        {
            _logger.LogWarning("TÜFE serisi boş döndü; içe aktarım atlandı.");
            return;
        }

        var existing = await _db.InflationIndices.ToDictionaryAsync(x => x.YearMonth, ct);

        foreach (var (ym, cpi) in series)
        {
            if (existing.TryGetValue(ym, out var row))
                row.CpiValue = cpi;
            else
                _db.InflationIndices.Add(new InflationIndex { YearMonth = ym, CpiValue = cpi });
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("TÜFE içe aktarıldı: {Count} ay", series.Count);
    }
}
