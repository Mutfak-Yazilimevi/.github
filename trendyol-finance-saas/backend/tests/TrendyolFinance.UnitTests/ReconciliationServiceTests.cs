using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using Xunit;

namespace TrendyolFinance.UnitTests;

public class ReconciliationServiceTests
{
    private static SettlementTransaction Sale(decimal gross, decimal commission, string barcode = "BRK1") => new()
    {
        TrendyolTransactionId = Guid.NewGuid().ToString(),
        TransactionType = SettlementTransactionType.Sale,
        Barcode = barcode,
        GrossAmount = gross,
        CommissionAmount = commission,
        TransactionDate = DateTimeOffset.UtcNow
    };

    [Fact]
    public void Dogru_komisyonda_bulgu_uretmez()
    {
        var svc = new ReconciliationService();
        // 100 × %12 = 12 beklenen, gerçek 12 → sapma yok
        var findings = svc.Check(new[] { Sale(100m, 12m) }, _ => 12m).ToList();
        Assert.Empty(findings);
    }

    [Fact]
    public void Buyuk_sapmada_critical_isaretler()
    {
        var svc = new ReconciliationService(warningThreshold: 0.5m, criticalThreshold: 2m);
        // beklenen 12, gerçek 20 → sapma 8 → Critical
        var findings = svc.Check(new[] { Sale(100m, 20m) }, _ => 12m).ToList();
        var f = Assert.Single(findings);
        Assert.Equal(ReconciliationSeverity.Critical, f.Severity);
        Assert.Equal(12m, f.ExpectedCommission);
        Assert.Equal(20m, f.ActualCommission);
    }

    [Fact]
    public void Oran_bilinmiyorsa_sessiz_gecer()
    {
        var svc = new ReconciliationService();
        var findings = svc.Check(new[] { Sale(100m, 999m) }, _ => null).ToList();
        Assert.Empty(findings);
    }
}
