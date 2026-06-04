using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;

namespace TrendyolFinance.Integration.Accounting;

public class ParasutOptions
{
    public const string SectionName = "Parasut";
    public string BaseUrl { get; set; } = "https://api.parasut.com/v4/";
    public string? CompanyId { get; set; }
    public string? AccessToken { get; set; }
}

/// <summary>
/// Paraşüt muhasebe entegrasyonu adaptörü (COGS kaynağı: AccountingIntegration).
/// NOT: Paraşüt OAuth2 ve uç noktaları canlı doğrulanacak — bu bir iskelet.
/// </summary>
public class ParasutAccountingProvider : IAccountingProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<ParasutAccountingProvider> _logger;

    public ParasutAccountingProvider(HttpClient http, ILogger<ParasutAccountingProvider> logger)
    {
        _http = http;
        _logger = logger;
    }

    public string Name => "Parasut";

    public Task<IReadOnlyList<AccountingCostRecord>> GetPurchaseCostsAsync(
        DateOnly since, CancellationToken ct = default)
    {
        // TODO: Paraşüt alış faturalarından (purchase invoices) ürün maliyetlerini çek.
        // OAuth2 token akışı + sayfalama eklenecek.
        _logger.LogInformation("Paraşüt alış maliyeti çekme henüz uygulanmadı (iskelet).");
        return Task.FromResult<IReadOnlyList<AccountingCostRecord>>(Array.Empty<AccountingCostRecord>());
    }
}
