using TrendyolFinance.Domain.Finance;

namespace TrendyolFinance.Application.Finance;

/// <summary>Tek bir hakediş kalemi için mutabakat bulgusu (F3).</summary>
public record ReconciliationFinding(
    string TrendyolTransactionId,
    string? OrderNumber,
    decimal ExpectedCommission,
    decimal ActualCommission,
    decimal Deviation,
    ReconciliationSeverity Severity);

public enum ReconciliationSeverity
{
    Ok = 0,
    Warning = 1,    // küçük sapma
    Critical = 2    // eşik üstü sapma
}

/// <summary>
/// Trendyol'un kestiği komisyonu bağımsız olarak doğrular (F3).
/// Beklenen = brüt × kategori komisyon oranı. Eşik üstü sapmaları işaretler.
/// UYARI: Kampanya komisyonu/özel anlaşmalar oranı değiştirir; MVP "büyük sapma" yakalar.
/// </summary>
public class ReconciliationService
{
    private readonly decimal _warningThreshold;
    private readonly decimal _criticalThreshold;

    public ReconciliationService(decimal warningThreshold = 0.50m, decimal criticalThreshold = 2.00m)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }

    /// <param name="expectedRateResolver">Barkod/kategori için beklenen komisyon oranını (%) döndürür.</param>
    public IEnumerable<ReconciliationFinding> Check(
        IEnumerable<SettlementTransaction> sales,
        Func<string?, decimal?> expectedRateResolver)
    {
        foreach (var t in sales)
        {
            if (t.TransactionType != SettlementTransactionType.Sale) continue;

            var expectedRate = expectedRateResolver(t.Barcode);
            if (expectedRate is null) continue; // oran bilinmiyorsa sessiz geç

            var expectedCommission = Math.Round(t.GrossAmount * expectedRate.Value / 100m, 2);
            var deviation = Math.Abs(t.CommissionAmount - expectedCommission);

            var severity = deviation >= _criticalThreshold ? ReconciliationSeverity.Critical
                         : deviation >= _warningThreshold ? ReconciliationSeverity.Warning
                         : ReconciliationSeverity.Ok;

            if (severity != ReconciliationSeverity.Ok)
            {
                yield return new ReconciliationFinding(
                    t.TrendyolTransactionId, t.OrderNumber,
                    expectedCommission, t.CommissionAmount, deviation, severity);
            }
        }
    }
}
