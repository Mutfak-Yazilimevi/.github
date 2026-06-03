namespace TrendyolFinance.Application.Analytics;

public enum RecommendationKind
{
    PriceIncrease,      // reel fiyat eridi → zam öner
    DeadStockAction,    // ölü stok → indirim/kampanya
    MarginWarning,      // düşük/negatif marj
    ReconciliationAlert // hakediş sapması
}

public record Recommendation(
    RecommendationKind Kind,
    string Title,
    string Detail,
    int Priority);      // 1 (yüksek) .. 5 (düşük)

/// <summary>
/// Analitik çıktıları üstüne kural tabanlı pazarlama/fiyat önerileri (F7).
/// Girdiler diğer servislerce önceden hesaplanır; bu servis saf karar mantığıdır.
/// </summary>
public class RecommendationService
{
    public IReadOnlyList<Recommendation> Build(
        IEnumerable<(string Barcode, decimal ErosionPercent)> priceErosions,
        IEnumerable<DeadStockItem> deadStock,
        decimal overallMarginPercent,
        int criticalReconciliationCount)
    {
        var recs = new List<Recommendation>();

        foreach (var (barcode, erosion) in priceErosions.Where(e => e.ErosionPercent <= -10))
        {
            recs.Add(new Recommendation(
                RecommendationKind.PriceIncrease,
                $"Zam değerlendir: {barcode}",
                $"Reel fiyat enflasyona göre %{Math.Abs(erosion):0.#} eridi. Alım gücünü korumak için fiyat güncellemesi önerilir.",
                Priority: 2));
        }

        foreach (var d in deadStock.Where(d => (d.TiedCapital ?? 0) > 0).Take(10))
        {
            recs.Add(new Recommendation(
                RecommendationKind.DeadStockAction,
                $"Ölü stok: {d.Barcode}",
                $"{d.DaysSinceLastSale} gündür satış yok, {d.Stock} adet ({d.TiedCapital:0.##} TL bağlı sermaye). İndirim/kampanya veya tasfiye düşünün.",
                Priority: 3));
        }

        if (overallMarginPercent < 5)
        {
            recs.Add(new Recommendation(
                RecommendationKind.MarginWarning,
                "Düşük kâr marjı",
                $"Genel net marj %{overallMarginPercent:0.#}. Maliyet/komisyon/iade kalemlerini gözden geçirin.",
                Priority: overallMarginPercent < 0 ? 1 : 2));
        }

        if (criticalReconciliationCount > 0)
        {
            recs.Add(new Recommendation(
                RecommendationKind.ReconciliationAlert,
                "Hakediş sapması",
                $"{criticalReconciliationCount} kalemde kritik komisyon sapması var. Trendyol hakedişlerini kontrol edin.",
                Priority: 1));
        }

        return recs.OrderBy(r => r.Priority).ToList();
    }
}
