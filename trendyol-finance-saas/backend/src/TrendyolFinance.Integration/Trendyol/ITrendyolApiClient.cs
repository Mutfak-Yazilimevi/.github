using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Integration.Trendyol.Models;

namespace TrendyolFinance.Integration.Trendyol;

/// <summary>Trendyol finans/katalog uç noktalarına erişim soyutlaması.</summary>
public interface ITrendyolApiClient
{
    /// <summary>Belirtilen pencere için tek sayfa hakediş kalemi getirir.</summary>
    Task<SettlementResponse> GetSettlementsAsync(
        TrendyolCredentials credentials,
        SettlementQuery query,
        CancellationToken ct = default);

    /// <summary>Bir pencere için tüm sayfaları otomatik dolaşarak kalemleri akış halinde verir.</summary>
    IAsyncEnumerable<SettlementItemDto> StreamSettlementsAsync(
        TrendyolCredentials credentials,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken ct = default);
}
