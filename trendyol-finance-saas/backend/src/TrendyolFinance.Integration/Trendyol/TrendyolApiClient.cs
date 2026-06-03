using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrendyolFinance.Integration.Trendyol.Models;

namespace TrendyolFinance.Integration.Trendyol;

/// <summary>
/// Trendyol Marketplace API istemcisi (finans odaklı).
/// Kimlik doğrulama: Basic Auth (ApiKey:ApiSecret). User-Agent zorunlu: "{SellerId} - SelfIntegration".
/// NOT: Uç nokta yolları ve alan adları resmî dokümandan teyit edilecek (portal bu ortamdan 403 verdi).
/// </summary>
public class TrendyolApiClient : ITrendyolApiClient
{
    private readonly HttpClient _http;
    private readonly TrendyolOptions _options;
    private readonly ILogger<TrendyolApiClient> _logger;

    public TrendyolApiClient(HttpClient http, IOptions<TrendyolOptions> options, ILogger<TrendyolApiClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SettlementResponse> GetSettlementsAsync(
        TrendyolCredentials credentials, SettlementQuery query, CancellationToken ct = default)
    {
        var url = $"finance/che/sellers/{credentials.SellerId}/settlements" +
                  $"?startDate={query.StartDate.ToUnixTimeMilliseconds()}" +
                  $"&endDate={query.EndDate.ToUnixTimeMilliseconds()}" +
                  $"&page={query.Page}&size={_options.PageSize}" +
                  (query.TransactionType is null ? "" : $"&transactionType={query.TransactionType}");

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyAuth(request, credentials);

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SettlementResponse>(cancellationToken: ct);
        return result ?? new SettlementResponse();
    }

    public async IAsyncEnumerable<SettlementItemDto> StreamSettlementsAsync(
        TrendyolCredentials credentials, DateTimeOffset start, DateTimeOffset end,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // Pencereyi azami genişliğe göre dilimle (API tarih aralığı limiti).
        foreach (var (windowStart, windowEnd) in SliceWindows(start, end, _options.MaxSettlementWindowDays))
        {
            var page = 0;
            int totalPages;
            do
            {
                var response = await GetSettlementsAsync(
                    credentials, new SettlementQuery(windowStart, windowEnd, Page: page), ct);

                foreach (var item in response.Content)
                    yield return item;

                totalPages = Math.Max(response.TotalPages, 1);
                page++;
            }
            while (page < totalPages && !ct.IsCancellationRequested);
        }
    }

    private void ApplyAuth(HttpRequestMessage request, TrendyolCredentials credentials)
    {
        var token = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{credentials.ApiKey}:{credentials.ApiSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        request.Headers.UserAgent.ParseAdd($"{credentials.SellerId} - SelfIntegration");
    }

    internal static IEnumerable<(DateTimeOffset Start, DateTimeOffset End)> SliceWindows(
        DateTimeOffset start, DateTimeOffset end, int maxDays)
    {
        var cursor = start;
        while (cursor < end)
        {
            var next = cursor.AddDays(maxDays);
            if (next > end) next = end;
            yield return (cursor, next);
            cursor = next;
        }
    }
}
