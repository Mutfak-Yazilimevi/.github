using LeatherErp.Application.Dtos;
using LeatherErp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Services;

/// <summary>Deri stok seviyelerini özetleyen ve düşük stok uyarısı üreten sorgu servisi.</summary>
public class StockService
{
    private readonly IAppDbContext _db;

    public StockService(IAppDbContext db) => _db = db;

    /// <summary>Her deri tipi için toplam kalan miktarı ve değerini döndürür.</summary>
    public async Task<List<StockLevelDto>> GetStockLevelsAsync(CancellationToken ct = default)
    {
        var types = await _db.LeatherTypes.AsNoTracking().ToListAsync(ct);
        var lots = await _db.LeatherLots.AsNoTracking().Where(l => l.RemainingDm2 > 0).ToListAsync(ct);

        return types.Select(t =>
        {
            var typeLots = lots.Where(l => l.LeatherTypeId == t.Id).ToList();
            return new StockLevelDto
            {
                LeatherTypeId = t.Id,
                LeatherTypeName = t.Name,
                RemainingDm2 = typeLots.Sum(l => l.RemainingDm2),
                StockValue = typeLots.Sum(l => l.RemainingDm2 * l.UnitCostPerDm2),
                LowStockThresholdDm2 = t.LowStockThresholdDm2
            };
        }).ToList();
    }

    /// <summary>Yalnızca eşiğin altına düşmüş deri tiplerini döndürür.</summary>
    public async Task<List<StockLevelDto>> GetLowStockAsync(CancellationToken ct = default)
        => (await GetStockLevelsAsync(ct)).Where(s => s.IsLow).ToList();
}
