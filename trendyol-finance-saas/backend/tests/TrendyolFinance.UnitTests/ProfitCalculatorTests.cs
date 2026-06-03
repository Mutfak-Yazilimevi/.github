using TrendyolFinance.Application.Finance;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Integration.Trendyol;
using Xunit;

namespace TrendyolFinance.UnitTests;

public class ProfitCalculatorTests
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
    public void Cogs_varken_net_kar_dogru_hesaplanir()
    {
        var calc = new ProfitCalculator();
        var txns = new[] { Sale(100m, 12m), Sale(200m, 24m) };

        // her satış için 40 TL birim maliyet
        var result = calc.Calculate(txns, (_, _) => 40m);

        Assert.Equal(300m, result.GrossRevenue);
        Assert.Equal(36m, result.Commission);
        Assert.Equal(80m, result.Cogs);
        Assert.Equal(264m, result.NetRevenue);     // 300 - 36
        Assert.Equal(184m, result.NetProfit);      // 264 - 80
        Assert.True(result.CogsAvailable);
    }

    [Fact]
    public void Cogs_eksikse_kar_guvenilmez_olarak_isaretlenir()
    {
        var calc = new ProfitCalculator();
        var result = calc.Calculate(new[] { Sale(100m, 12m) }, (_, _) => null);

        Assert.False(result.CogsAvailable);
        Assert.Equal(0m, result.NetProfit);        // COGS yokken kâr raporlanmaz
        Assert.Equal(88m, result.NetRevenue);      // net gelir yine de anlamlı
    }

    [Fact]
    public void Pencere_dilimleme_15_gunluk_parcalara_boler()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddDays(40);

        var windows = TrendyolApiClient.SliceWindows(start, end, 15).ToList();

        Assert.Equal(3, windows.Count);            // 15 + 15 + 10
        Assert.Equal(start, windows[0].Start);
        Assert.Equal(end, windows[^1].End);
    }
}
