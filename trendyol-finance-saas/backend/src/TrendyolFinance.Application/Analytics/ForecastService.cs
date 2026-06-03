namespace TrendyolFinance.Application.Analytics;

/// <summary>Aylık zaman serisi noktası (YYYYMM → değer).</summary>
public record SeriesPoint(int YearMonth, decimal Value);

public record ForecastPoint(int YearMonth, decimal Forecast);

public record ForecastResult(
    IReadOnlyList<SeriesPoint> History,
    IReadOnlyList<ForecastPoint> Forecast,
    decimal TrendPerMonth);   // doğrusal eğim (aylık değişim)

/// <summary>
/// Basit öngörü (F6): doğrusal trend + üstel düzleştirme karması.
/// MVP seviyesi; ileride sezonsallık ve ML.NET ile zenginleştirilir.
/// </summary>
public class ForecastService
{
    public ForecastResult Forecast(IReadOnlyList<SeriesPoint> history, int horizonMonths = 3, double alpha = 0.5)
    {
        var ordered = history.OrderBy(p => p.YearMonth).ToList();
        if (ordered.Count == 0)
            return new ForecastResult(ordered, Array.Empty<ForecastPoint>(), 0);

        // Üstel düzleştirilmiş seviye.
        decimal level = ordered[0].Value;
        foreach (var p in ordered.Skip(1))
            level = (decimal)alpha * p.Value + (decimal)(1 - alpha) * level;

        // Doğrusal trend (en küçük kareler, x = 0..n-1).
        var trend = LinearSlope(ordered.Select(p => (double)p.Value).ToList());

        var result = new List<ForecastPoint>(horizonMonths);
        var ym = ordered[^1].YearMonth;
        for (var h = 1; h <= horizonMonths; h++)
        {
            ym = NextYearMonth(ym);
            var forecast = level + (decimal)trend * h;
            result.Add(new ForecastPoint(ym, Math.Round(Math.Max(forecast, 0), 2)));
        }

        return new ForecastResult(ordered, result, Math.Round((decimal)trend, 2));
    }

    private static double LinearSlope(IReadOnlyList<double> y)
    {
        var n = y.Count;
        if (n < 2) return 0;
        double sx = 0, sy = 0, sxy = 0, sxx = 0;
        for (var i = 0; i < n; i++)
        {
            sx += i; sy += y[i]; sxy += i * y[i]; sxx += (double)i * i;
        }
        var denom = n * sxx - sx * sx;
        return denom == 0 ? 0 : (n * sxy - sx * sy) / denom;
    }

    internal static int NextYearMonth(int ym)
    {
        var year = ym / 100;
        var month = ym % 100;
        return month == 12 ? (year + 1) * 100 + 1 : year * 100 + month + 1;
    }
}
