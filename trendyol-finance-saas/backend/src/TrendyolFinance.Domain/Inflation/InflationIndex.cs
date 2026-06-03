namespace TrendyolFinance.Domain.Inflation;

/// <summary>
/// TÜİK TÜFE endeksi (aylık). Tenant'tan bağımsız, paylaşımlı referans verisi.
/// Reel fiyat = NominalFiyat × (CPI_baz / CPI_t).
/// </summary>
public class InflationIndex
{
    /// <summary>Yıl-ay anahtarı (örn. 202606).</summary>
    public int YearMonth { get; set; }

    /// <summary>TÜFE endeks değeri.</summary>
    public decimal CpiValue { get; set; }

    public string Source { get; set; } = "TUIK";
}
