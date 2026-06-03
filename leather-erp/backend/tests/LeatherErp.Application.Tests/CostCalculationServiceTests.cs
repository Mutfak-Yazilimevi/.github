using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using Xunit;

namespace LeatherErp.Application.Tests;

public class CostCalculationServiceTests
{
    private readonly CostCalculationService _sut = new();

    [Fact]
    public void Calculate_AddsWasteOnTopOfNetLeather()
    {
        // 8 dm² net, %15 fire → brüt 9.2 dm²
        var result = _sut.Calculate(new CostInput
        {
            NetLeatherDm2 = 8m,
            WasteRate = 0.15m,
            UnitCostPerDm2 = 12.50m,
            LaborCost = 40m,
            OverheadCost = 15m
        });

        Assert.Equal(9.2m, result.GrossLeatherDm2);
        Assert.Equal(100m, result.NetMaterialCost);          // 8 × 12.50
        Assert.Equal(115m, result.MaterialCost);             // 9.2 × 12.50
        Assert.Equal(15m, result.WasteCost);                 // fire'ın ek maliyeti
        Assert.Equal(170m, result.UnitCost);                 // 115 + 40 + 15
    }

    [Fact]
    public void Calculate_WithZeroWaste_HasNoWasteCost()
    {
        var result = _sut.Calculate(new CostInput
        {
            NetLeatherDm2 = 10m,
            WasteRate = 0m,
            UnitCostPerDm2 = 10m,
            LaborCost = 0m,
            OverheadCost = 0m
        });

        Assert.Equal(10m, result.GrossLeatherDm2);
        Assert.Equal(0m, result.WasteCost);
        Assert.Equal(100m, result.UnitCost);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(1.5)]
    public void Calculate_InvalidWasteRate_Throws(decimal wasteRate)
    {
        Assert.Throws<ArgumentException>(() => _sut.Calculate(new CostInput
        {
            NetLeatherDm2 = 1m,
            WasteRate = wasteRate,
            UnitCostPerDm2 = 1m
        }));
    }
}
