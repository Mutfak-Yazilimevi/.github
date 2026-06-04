using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrendyolFinance.Application.Abstractions;

namespace TrendyolFinance.Integration.Tuik;

public class TuikOptions
{
    public const string SectionName = "Tuik";

    /// <summary>TCMB EVDS API taban adresi.</summary>
    public string BaseUrl { get; set; } = "https://evds2.tcmb.gov.tr/service/evds/";

    /// <summary>EVDS API anahtarı (kullanıcı kendi anahtarını girer).</summary>
    public string? ApiKey { get; set; }

    /// <summary>TÜFE genel endeks serisi kodu (EVDS).</summary>
    public string SeriesCode { get; set; } = "TP.FG.J0";
}

/// <summary>
/// TCMB EVDS üzerinden TÜFE endeksini çeker.
/// NOT: EVDS yanıt şeması ve seri kodu canlı doğrulanmalı (anahtar gerektirir).
/// </summary>
public class TuikEvdsInflationProvider : IInflationProvider
{
    private readonly HttpClient _http;
    private readonly TuikOptions _options;
    private readonly ILogger<TuikEvdsInflationProvider> _logger;

    public TuikEvdsInflationProvider(
        HttpClient http, IOptions<TuikOptions> options, ILogger<TuikEvdsInflationProvider> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<(int YearMonth, decimal Cpi)>> GetCpiSeriesAsync(
        int fromYearMonth, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("EVDS API anahtarı tanımlı değil; TÜFE çekilemedi.");
            return Array.Empty<(int, decimal)>();
        }

        var startMonth = $"01-{(fromYearMonth % 100):00}-{fromYearMonth / 100}";
        var url = $"series={_options.SeriesCode}&startDate={startMonth}" +
                  $"&endDate=01-{DateTime.UtcNow:MM-yyyy}&type=json&key={_options.ApiKey}";

        var response = await _http.GetFromJsonAsync<EvdsResponse>(url, ct);
        if (response?.Items is null) return Array.Empty<(int, decimal)>();

        var result = new List<(int, decimal)>();
        foreach (var item in response.Items)
        {
            // EVDS tarih biçimi: "2026-6" benzeri.
            if (item.Date is null) continue;
            var parts = item.Date.Split('-');
            if (parts.Length < 2) continue;
            if (!int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month)) continue;

            var raw = item.Value;
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var cpi))
            {
                result.Add((year * 100 + month, cpi));
            }
        }
        return result;
    }

    private sealed class EvdsResponse
    {
        public List<EvdsItem>? Items { get; set; }
    }

    private sealed class EvdsItem
    {
        public string? Date { get; set; }
        // EVDS alan adı seri koduna göre değişir; ham metin olarak alınır.
        public string? Value { get; set; }
    }
}
