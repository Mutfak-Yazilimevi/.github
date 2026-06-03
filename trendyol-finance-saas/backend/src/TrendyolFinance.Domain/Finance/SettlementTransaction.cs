using TrendyolFinance.Domain.Common;

namespace TrendyolFinance.Domain.Finance;

/// <summary>
/// Trendyol hakediş (settlement) kalemi — kârlılık ve mutabakatın çekirdek ham verisi.
/// TrendyolTransactionId + SellerAccountId benzersizdir (idempotent upsert).
/// </summary>
public class SettlementTransaction : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Guid SellerAccountId { get; set; }

    /// <summary>Trendyol tarafındaki kalem kimliği (dedup anahtarı).</summary>
    public required string TrendyolTransactionId { get; set; }

    public SettlementTransactionType TransactionType { get; set; }

    public string? OrderNumber { get; set; }
    public string? Barcode { get; set; }

    public DateTimeOffset TransactionDate { get; set; }

    /// <summary>Brüt satış/işlem tutarı.</summary>
    public decimal GrossAmount { get; set; }

    /// <summary>Trendyol'un uyguladığı komisyon oranı (% — örn. 12.5).</summary>
    public decimal CommissionRate { get; set; }

    /// <summary>Trendyol'un kestiği komisyon tutarı.</summary>
    public decimal CommissionAmount { get; set; }

    /// <summary>Satıcının net kazancı (Trendyol'un raporladığı).</summary>
    public decimal SellerRevenue { get; set; }

    public string Currency { get; set; } = "TRY";

    /// <summary>Ham yanıt (denetim/yeniden işleme için).</summary>
    public string? RawJson { get; set; }

    public DateTimeOffset IngestedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Trendyol hakediş kalem tipleri (mutabakatın temeli).</summary>
public enum SettlementTransactionType
{
    Unknown = 0,
    Sale = 1,
    Return = 2,
    Discount = 3,
    DiscountCancel = 4,
    Coupon = 5,
    CouponCancel = 6,
    ProvisionPositive = 7,
    ProvisionNegative = 8,
    ManualRefund = 9,
    ManualRefundCancel = 10,
    Stoppage = 11,        // tevkifat/kesinti
    CommissionAgreement = 12
}
