using LeatherErp.Application.Dtos;

namespace LeatherErp.Application.Services;

/// <summary>
/// Fiyatlandırma: maliyet üzerine kâr marjı (markup) ve KDV uygulayarak satış fiyatı üretir.
/// salePrice = unitCost × (1 + profitMargin) × (1 + vatRate).
/// </summary>
public class PricingService
{
    /// <summary>İleri yön: maliyetten KDV dahil satış fiyatına.</summary>
    public PricingResult Calculate(PricingInput input)
    {
        if (input.UnitCost < 0) throw new ArgumentException("Birim maliyet negatif olamaz.", nameof(input));
        if (input.ProfitMargin < 0) throw new ArgumentException("Kâr marjı negatif olamaz.", nameof(input));
        if (input.VatRate < 0) throw new ArgumentException("KDV oranı negatif olamaz.", nameof(input));

        var profit = input.UnitCost * input.ProfitMargin;
        var priceBeforeTax = input.UnitCost + profit;
        var vat = priceBeforeTax * input.VatRate;
        var salePrice = priceBeforeTax + vat;

        return new PricingResult
        {
            UnitCost = input.UnitCost,
            ProfitAmount = profit,
            PriceBeforeTax = priceBeforeTax,
            VatAmount = vat,
            SalePrice = salePrice
        };
    }

    /// <summary>
    /// Ters yön: hedeflenen KDV dahil satış fiyatından, maliyete göre gerçekleşen kâr marjını çözer.
    /// margin = (priceBeforeTax / unitCost) - 1.
    /// </summary>
    public decimal SolveProfitMargin(ReversePricingInput input)
    {
        if (input.UnitCost <= 0) throw new ArgumentException("Birim maliyet sıfırdan büyük olmalıdır.", nameof(input));
        if (input.VatRate < 0) throw new ArgumentException("KDV oranı negatif olamaz.", nameof(input));

        var priceBeforeTax = input.TargetSalePrice / (1 + input.VatRate);
        return (priceBeforeTax / input.UnitCost) - 1;
    }
}
