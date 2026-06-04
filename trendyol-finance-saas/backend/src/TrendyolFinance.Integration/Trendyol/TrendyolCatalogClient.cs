using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        var token = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{credentials.ApiKey}:{credentials.ApiSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        request.Headers.UserAgent.ParseAdd($"{credentials.SellerId} - SelfIntegration");

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CommissionsResponse>(cancellationToken: ct);
        return payload?.Categories?
            .Select(c => new CategoryCommissionDto(c.CategoryId, c.Name, c.CommissionRate))
            .ToList()
            ?? new List<CategoryCommissionDto>();
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
}
