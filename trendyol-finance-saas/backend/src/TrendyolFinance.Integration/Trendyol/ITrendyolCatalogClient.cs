using TrendyolFinance.Application.Abstractions;

namespace TrendyolFinance.Integration.Trendyol;

/// <summary>Trendyol kategori komisyon oranı kaydı.</summary>
public record CategoryCommissionDto(int CategoryId, string? CategoryName, decimal CommissionRate);

/// <summary>Trendyol katalog/komisyon uç noktaları (mutabakat referans verisi için).</summary>
public interface ITrendyolCatalogClient
{
    /// <summary>Kategori bazında komisyon oranlarını getirir.</summary>
    Task<IReadOnlyList<CategoryCommissionDto>> GetCategoryCommissionsAsync(
        TrendyolCredentials credentials, CancellationToken ct = default);
}
