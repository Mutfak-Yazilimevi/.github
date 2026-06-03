using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Integration.Trendyol.Models;

namespace TrendyolFinance.Ingestion;

/// <summary>Trendyol settlement DTO'sunu alan modeline çevirir.</summary>
public static class SettlementMapper
{
    public static SettlementTransaction ToEntity(SettlementItemDto dto, Guid tenantId, Guid sellerAccountId) => new()
    {
        TenantId = tenantId,
        SellerAccountId = sellerAccountId,
        TrendyolTransactionId = dto.Id,
        TransactionType = MapType(dto.TransactionType),
        OrderNumber = dto.OrderNumber,
        Barcode = dto.Barcode,
        TransactionDate = DateTimeOffset.FromUnixTimeMilliseconds(dto.TransactionDate),
        GrossAmount = dto.Credit - dto.Debt,
        CommissionRate = dto.CommissionRate,
        CommissionAmount = dto.CommissionAmount,
        SellerRevenue = dto.SellerRevenue,
        Currency = dto.Currency,
        IngestedAt = DateTimeOffset.UtcNow
    };

    public static SettlementTransactionType MapType(string? raw) => raw?.Trim().ToLowerInvariant() switch
    {
        "sale" => SettlementTransactionType.Sale,
        "return" => SettlementTransactionType.Return,
        "discount" => SettlementTransactionType.Discount,
        "discountcancel" => SettlementTransactionType.DiscountCancel,
        "coupon" => SettlementTransactionType.Coupon,
        "couponcancel" => SettlementTransactionType.CouponCancel,
        "provisionpositive" => SettlementTransactionType.ProvisionPositive,
        "provisionnegative" => SettlementTransactionType.ProvisionNegative,
        "manualrefund" => SettlementTransactionType.ManualRefund,
        "manualrefundcancel" => SettlementTransactionType.ManualRefundCancel,
        "stoppage" => SettlementTransactionType.Stoppage,
        "commissionagreement" => SettlementTransactionType.CommissionAgreement,
        _ => SettlementTransactionType.Unknown
    };
}
