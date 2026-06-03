using LeatherErp.Application.Dtos;

namespace LeatherErp.Application.Services;

/// <summary>
/// Birim maliyet hesabı. Fire konvansiyonu: fire oranı NET deri üzerine EKLENİR
/// (brüt = net × (1 + fire)). Böylece "%15 fire" girdiğinizde, 100 dm² net için 115 dm² deri tüketilir.
/// </summary>
public class CostCalculationService
{
    /// <summary>Verilen girdiden birim maliyetin şeffaf kırılımını üretir.</summary>
    public CostBreakdown Calculate(CostInput input)
    {
        if (input.NetLeatherDm2 < 0) throw new ArgumentException("Net deri miktarı negatif olamaz.", nameof(input));
        if (input.WasteRate < 0 || input.WasteRate >= 1) throw new ArgumentException("Fire oranı [0,1) aralığında olmalıdır.", nameof(input));
        if (input.UnitCostPerDm2 < 0) throw new ArgumentException("Deri birim maliyeti negatif olamaz.", nameof(input));

        var grossLeather = input.NetLeatherDm2 * (1 + input.WasteRate);
        var netMaterial = input.NetLeatherDm2 * input.UnitCostPerDm2;
        var materialCost = grossLeather * input.UnitCostPerDm2;
        var wasteCost = materialCost - netMaterial;
        var unitCost = materialCost + input.LaborCost + input.OverheadCost;

        return new CostBreakdown
        {
            GrossLeatherDm2 = grossLeather,
            NetMaterialCost = netMaterial,
            WasteCost = wasteCost,
            MaterialCost = materialCost,
            LaborCost = input.LaborCost,
            OverheadCost = input.OverheadCost,
            UnitCost = unitCost
        };
    }
}
