using TrendyolFinance.Application.Analytics;
using Xunit;

namespace TrendyolFinance.UnitTests;

public class ForecastServiceTests
{
    [Fact]
    public void Artan_seriyi_yukari_trendle_tahmin_eder()
    {
        var history = new List<SeriesPoint>
        {
            new(202601, 100m), new(202602, 120m), new(202603, 140m),
            new(202604, 160m), new(202605, 180m),
        };

        var result = new ForecastService().Forecast(history, horizonMonths: 2);

        Assert.Equal(2, result.Forecast.Count);
        Assert.Equal(202606, result.Forecast[0].YearMonth);   // sonraki ay
        Assert.True(result.TrendPerMonth > 0);                 // artan trend
        Assert.True(result.Forecast[1].Forecast >= result.Forecast[0].Forecast);
    }

    [Theory]
    [InlineData(202612, 202701)]   // yıl atlama
    [InlineData(202605, 202606)]
    public void Sonraki_ay_dogru_hesaplanir(int input, int expected) =>
        Assert.Equal(expected, ForecastService.NextYearMonth(input));

    [Fact]
    public void Bos_seri_bos_tahmin_dondurur()
    {
        var result = new ForecastService().Forecast(new List<SeriesPoint>());
        Assert.Empty(result.Forecast);
    }
}
