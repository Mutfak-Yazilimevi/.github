using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;

namespace TrendyolFinance.Integration.Trendyol;

/// <summary>
/// Trendyol katalog/komisyon istemcisi. Basic Auth + zorunlu User-Agent.
/// NOT: Uç nokta yolu ve yanıt şeması resmî dokümandan teyit edilecek.
/// </summary>
public class TrendyolCatalogClient : ITrendyolCatalogClient
{
    private readonly HttpClient _http;
    private readonly ILogger<TrendyolCatalogClient> _logger;

    public TrendyolCatalogClient(HttpClient http, ILogger<TrendyolCatalogClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CategoryCommissionDto>> GetCategoryCommissionsAsync(
        TrendyolCredentials credentials, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "product/product-categories/commissions");
        ApplyAuth(request, credentials);

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CommissionsResponse>(cancellationToken: ct);
        return payload?.Categories?
            .Select(c => new CategoryCommissionDto(c.CategoryId, c.Name, c.CommissionRate))
            .ToList()
            ?? new List<CategoryCommissionDto>();
    }

    public async IAsyncEnumerable<ProductDto> StreamProductsAsync(
        TrendyolCredentials credentials, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = 0;
        int totalPages;
        do
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"product/sellers/{credentials.SellerId}/products?page={page}&size=200");
            ApplyAuth(request, credentials);

            using var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<ProductsResponse>(cancellationToken: ct);
            foreach (var p in payload?.Content ?? new())
            {
                if (string.IsNullOrWhiteSpace(p.Barcode)) continue;
                yield return new ProductDto(
                    p.Barcode!, p.StockCode, p.Title ?? p.Barcode!, p.PimCategoryId, p.CategoryName,
                    p.Brand, p.ListPrice, p.SalePrice, p.Quantity);
            }

            totalPages = Math.Max(payload?.TotalPages ?? 1, 1);
            page++;
        }
        while (page < totalPages && !ct.IsCancellationRequested);
    }

    private void ApplyAuth(HttpRequestMessage request, TrendyolCredentials credentials)
    {
        var token = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{credentials.ApiKey}:{credentials.ApiSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        request.Headers.UserAgent.ParseAdd($"{credentials.SellerId} - SelfIntegration");
    }

    private sealed class CommissionsResponse
    {
        public List<CommissionItem>? Categories { get; set; }
    }

    private sealed class CommissionItem
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public decimal CommissionRate { get; set; }
    }

    private sealed class ProductsResponse
    {
        public int TotalPages { get; set; }
        public List<ProductItem> Content { get; set; } = new();
    }

    private sealed class ProductItem
    {
        public string? Barcode { get; set; }
        public string? StockCode { get; set; }
        public string? Title { get; set; }
        public int? PimCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Brand { get; set; }
        public decimal ListPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
    }
}
