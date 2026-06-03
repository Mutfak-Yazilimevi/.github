using LeatherErp.Domain.Common;
using LeatherErp.Domain.Enums;
using Xunit;

namespace LeatherErp.Application.Tests;

public class UnitConverterTests
{
    [Fact]
    public void ToDm2_FromSquareFoot_MultipliesByConstant()
        => Assert.Equal(92.903m, UnitConverter.ToDm2(10m, UnitOfMeasure.SquareFoot));

    [Fact]
    public void FromDm2_ToSquareFoot_DividesByConstant()
        => Assert.Equal(10m, Math.Round(UnitConverter.FromDm2(92.903m, UnitOfMeasure.SquareFoot), 4));

    [Fact]
    public void Dm2_IsIdentity()
    {
        Assert.Equal(50m, UnitConverter.ToDm2(50m, UnitOfMeasure.Dm2));
        Assert.Equal(50m, UnitConverter.FromDm2(50m, UnitOfMeasure.Dm2));
    }

    [Fact]
    public void Convert_RoundTrip_PreservesValue()
    {
        var dm2 = UnitConverter.Convert(7.5m, UnitOfMeasure.SquareFoot, UnitOfMeasure.Dm2);
        var backToSqft = UnitConverter.Convert(dm2, UnitOfMeasure.Dm2, UnitOfMeasure.SquareFoot);
        Assert.Equal(7.5m, Math.Round(backToSqft, 6));
    }
}
