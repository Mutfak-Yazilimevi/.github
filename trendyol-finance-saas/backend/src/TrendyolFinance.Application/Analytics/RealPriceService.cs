using TrendyolFinance.Domain.Catalog;

namespace TrendyolFinance.Application.Analytics;

public record RealPricePoint(DateOnly Date, decimal NominalPrice, decimal RealPrice);

public record RealPriceResult(
    decimal BasePrice,
    decimal CurrentNominal,
    decimal CurrentReal,
    decimal ErosionPercent,    // negatif = alım gücü olarak eridi
    IReadOnlyList<RealPricePoint> Series);

/// <summary>
/// Enflasyona göre reel fiyat erimesini hesaplar (F4).
/// Reel(t) = Nominal(t) × (CPI_baz / CPI_t). Erime = (Reel_son − Fiyat_baz) / Fiyat_baz.
/// </summary>
public class RealPriceService
{
    /// <param name="cpiByYearMonth">YYYYMM → TÜFE endeksi.</param>
    public RealPriceResult? Compute(
        IReadOnlyList<PriceSnapshot> snapshots,
        IReadOnlyDictionary<int, decimal> cpiByYearMonth)
    {
        var ordered = snapshots.OrderBy(s => s.CapturedOn).ToList();
        if (ordered.Count == 0) return null;

        var baseSnap = ordered[0];
        var baseYm = YearMonth(baseSnap.CapturedOn);
        if (!cpiByYearMonth.TryGetValue(baseYm, out var baseCpi) || baseCpi == 0) return null;

        var basePrice = baseSnap.SalePrice;
        var series = new List<RealPricePoint>(ordered.Count);

        foreach (var s in ordered)
        {
            var ym = YearMonth(s.CapturedOn);
            if (!cpiByYearMonth.TryGetValue(ym, out var cpi) || cpi == 0) continue;
            var real = Math.Round(s.SalePrice * (baseCpi / cpi), 2);
            series.Add(new RealPricePoint(s.CapturedOn, s.SalePrice, real));
        }

        if (series.Count == 0) return null;

        var last = series[^1];
        var erosion = basePrice == 0 ? 0 : Math.Round((last.RealPrice - basePrice) / basePrice * 100, 2);

        return new RealPriceResult(basePrice, last.NominalPrice, last.RealPrice, erosion, series);
    }

    private static int YearMonth(DateOnly d) => d.Year * 100 + d.Month;
}
