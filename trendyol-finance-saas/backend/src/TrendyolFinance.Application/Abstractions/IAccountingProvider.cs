namespace TrendyolFinance.Application.Abstractions;

/// <summary>Muhasebe/e-fatura sisteminden gelen tek bir alış maliyeti kaydı.</summary>
public record AccountingCostRecord(
    string Barcode,
    decimal PurchasePrice,
    decimal VatRate,
    DateOnly EffectiveFrom);

/// <summary>
/// Muhasebe entegrasyonu adaptörü (COGS'un üçüncü kaynağı → CostSource.AccountingIntegration).
/// Her hedef sistem (Paraşüt, Logo, Mikro...) bu arayüzü uygular.
/// </summary>
public interface IAccountingProvider
{
    /// <summary>Sağlayıcı adı (örn. "Parasut").</summary>
    string Name { get; }

    /// <summary>Belirtilen tarihten itibaren güncellenen alış maliyetlerini getirir.</summary>
    Task<IReadOnlyList<AccountingCostRecord>> GetPurchaseCostsAsync(
        DateOnly since, CancellationToken ct = default);
}
