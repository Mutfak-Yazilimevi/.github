namespace TrendyolFinance.Application.Analytics;

/// <summary>İşletme sağlık skoru girdileri (önceden hesaplanmış metrikler).</summary>
public record HealthInputs(
    decimal MarginPercent,       // net kâr marjı
    decimal MarginTrendPercent,  // önceki döneme göre marj değişimi
    decimal ReturnRatePercent,   // iade oranı
    decimal RevenueGrowthPercent,// ciro büyümesi
    decimal StockTurnover);      // stok devir hızı (yıllık)

public record HealthScore(
    int Score,                   // 0..100
    string Grade,                // A..F
    IReadOnlyDictionary<string, int> Breakdown);

/// <summary>"İşletmem iyi mi kötü mü?" sorusuna birleşik bir skor üretir (F2).</summary>
public class HealthScoreCalculator
{
    public HealthScore Calculate(HealthInputs i)
    {
        var breakdown = new Dictionary<string, int>
        {
            ["Kâr marjı"]      = Band(i.MarginPercent, 0, 25),
            ["Marj trendi"]    = Band(i.MarginTrendPercent, -10, 10),
            ["İade oranı"]     = Band(i.ReturnRatePercent, 20, 0),   // ters: düşük iade iyi
            ["Ciro büyümesi"]  = Band(i.RevenueGrowthPercent, -10, 20),
            ["Stok devri"]     = Band(i.StockTurnover, 0, 8),
        };

        // Ağırlıklı ortalama (her bileşen 0..100).
        var weights = new Dictionary<string, double>
        {
            ["Kâr marjı"] = 0.30, ["Marj trendi"] = 0.20, ["İade oranı"] = 0.15,
            ["Ciro büyümesi"] = 0.20, ["Stok devri"] = 0.15
        };

        var score = (int)Math.Round(breakdown.Sum(kv => kv.Value * weights[kv.Key]));
        return new HealthScore(score, ToGrade(score), breakdown);
    }

    /// <summary>Değeri [low..high] aralığına göre 0..100'e ölçekler (low→0, high→100). low>high ise ters çevirir.</summary>
    private static int Band(decimal value, decimal low, decimal high)
    {
        if (low == high) return 50;
        var t = (double)((value - low) / (high - low));
        return (int)Math.Round(Math.Clamp(t, 0, 1) * 100);
    }

    private static string ToGrade(int s) => s switch
    {
        >= 85 => "A",
        >= 70 => "B",
        >= 55 => "C",
        >= 40 => "D",
        _ => "F"
    };
}
