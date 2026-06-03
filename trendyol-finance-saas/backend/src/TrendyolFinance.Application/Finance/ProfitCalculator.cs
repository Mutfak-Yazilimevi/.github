using TrendyolFinance.Domain.Catalog;
using TrendyolFinance.Domain.Finance;

namespace TrendyolFinance.Application.Finance;

/// <summary>Bir dönem için kârlılık sonucu (F1/F2).</summary>
public record ProfitResult(
    decimal GrossRevenue,     // brüt satış
    decimal Commission,       // Trendyol komisyonu
    decimal Deductions,       // iade/iptal/kargo/hizmet kesintileri
    decimal Cogs,             // satılan malın maliyeti
    decimal NetRevenue,       // brüt − komisyon − kesinti (maliyet hariç)
    decimal NetProfit,        // net gelir − COGS
    bool CogsAvailable)       // maliyet girilmemişse kâr güvenilmezdir
{
    public decimal MarginPercent => GrossRevenue == 0 ? 0 : Math.Round(NetProfit / GrossRevenue * 100, 2);
}

/// <summary>
/// Hakediş kalemlerini ve (varsa) COGS'u birleştirerek gerçek kârı hesaplar.
/// COGS yoksa yalnızca net gelir anlamlıdır; UI bunu açıkça belirtmelidir.
/// </summary>
public class ProfitCalculator
{
    /// <param name="costResolver">Satış anındaki birim maliyeti döndürür; maliyet yoksa null.</param>
    public ProfitResult Calculate(
        IReadOnlyCollection<SettlementTransaction> transactions,
        Func<string?, DateTimeOffset, decimal?> costResolver)
    {
        decimal grossRevenue = 0, commission = 0, deductions = 0, cogs = 0;
        var anyCostMissing = false;
        var anySale = false;

        foreach (var t in transactions)
        {
            commission += t.CommissionAmount;

            switch (t.TransactionType)
            {
                case SettlementTransactionType.Sale:
                    grossRevenue += t.GrossAmount;
                    anySale = true;
                    var unitCost = costResolver(t.Barcode, t.TransactionDate);
                    if (unitCost is null) anyCostMissing = true;
                    else cogs += unitCost.Value;
                    break;

                case SettlementTransactionType.Return:
                case SettlementTransactionType.ManualRefund:
                    deductions += Math.Abs(t.GrossAmount);
                    break;

                case SettlementTransactionType.Discount:
                case SettlementTransactionType.Coupon:
                case SettlementTransactionType.Stoppage:
                    deductions += Math.Abs(t.GrossAmount);
                    break;
            }
        }

        var netRevenue = grossRevenue - commission - deductions;
        var cogsAvailable = anySale && !anyCostMissing;
        var netProfit = cogsAvailable ? netRevenue - cogs : 0m;

        return new ProfitResult(grossRevenue, commission, deductions, cogs, netRevenue, netProfit, cogsAvailable);
    }
}
