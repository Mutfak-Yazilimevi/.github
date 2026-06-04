using TrendyolFinance.Application.Abstractions;

namespace TrendyolFinance.Integration.Trendyol;

/// <summary>Trendyol kategori komisyon oranı kaydı.</summary>
public record CategoryCommissionDto(int CategoryId, string? CategoryName, decimal CommissionRate);

/// <summary>Trendyol ürün kaydı (katalog senkronu için).</summary>
public record ProductDto(
    string Barcode,
    string? StockCode,
    string Title,
    int? CategoryId,
    string? CategoryName,
    string? Brand,
    decimal ListPrice,
    decimal SalePrice,
    int Quantity);

/// <summary>Trendyol katalog/komisyon uç noktaları (mutabakat + katalog referans verisi için).</summary>
public interface ITrendyolCatalogClient
{
    /// <summary>Kategori bazında komisyon oranlarını getirir.</summary>
    Task<IReadOnlyList<CategoryCommissionDto>> GetCategoryCommissionsAsync(
        TrendyolCredentials credentials, CancellationToken ct = default);

    /// <summary>Mağazanın ürünlerini sayfalı olarak akış halinde getirir.</summary>
    IAsyncEnumerable<ProductDto> StreamProductsAsync(
        TrendyolCredentials credentials, CancellationToken ct = default);
}
