namespace TrendyolFinance.Application.Abstractions;

/// <summary>TÜFE (enflasyon) endeks serisini sağlar — reel fiyat hesabı (F4) için.</summary>
public interface IInflationProvider
{
    /// <returns>(YYYYMM, TÜFE endeksi) çiftleri, verilen aydan itibaren.</returns>
    Task<IReadOnlyList<(int YearMonth, decimal Cpi)>> GetCpiSeriesAsync(
        int fromYearMonth, CancellationToken ct = default);
}
