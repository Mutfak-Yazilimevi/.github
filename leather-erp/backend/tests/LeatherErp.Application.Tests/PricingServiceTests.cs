using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using Xunit;

namespace LeatherErp.Application.Tests;

public class PricingServiceTests
{
    private readonly PricingService _sut = new();

    [Fact]
    public void Calculate_AppliesMarginThenVat()
    {
        // 170 maliyet, %40 kâr → 238 (KDV hariç), %20 KDV → 285.60
        var result = _sut.Calculate(new PricingInput
        {
            UnitCost = 170m,
            ProfitMargin = 0.40m,
            VatRate = 0.20m
        });

        Assert.Equal(68m, result.ProfitAmount);
        Assert.Equal(238m, result.PriceBeforeTax);
        Assert.Equal(47.60m, result.VatAmount);
        Assert.Equal(285.60m, result.SalePrice);
    }

    [Fact]
    public void SolveProfitMargin_IsInverseOfCalculate()
    {
        var margin = _sut.SolveProfitMargin(new ReversePricingInput
        {
            UnitCost = 170m,
            TargetSalePrice = 285.60m,
            VatRate = 0.20m
        });

        Assert.Equal(0.40m, Math.Round(margin, 4));
    }

    [Fact]
    public void Calculate_NegativeCost_Throws()
        => Assert.Throws<ArgumentException>(() => _sut.Calculate(new PricingInput { UnitCost = -1m }));
}
