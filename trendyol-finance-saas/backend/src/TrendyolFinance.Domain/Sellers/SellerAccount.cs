using TrendyolFinance.Domain.Common;

namespace TrendyolFinance.Domain.Sellers;

/// <summary>
/// Bir tenant'a bağlı tek bir Trendyol mağazası. API kimlik bilgileri ŞİFRELİ saklanır
/// (bkz. Infrastructure katmanındaki şifreleme dönüştürücüsü).
/// </summary>
public class SellerAccount : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }

    public required string StoreName { get; set; }

    /// <summary>Trendyol Satıcı ID (SellerId / SupplierId).</summary>
    public required long TrendyolSellerId { get; set; }

    /// <summary>Şifrelenmiş API Key.</summary>
    public required string ApiKeyEncrypted { get; set; }

    /// <summary>Şifrelenmiş API Secret.</summary>
    public required string ApiSecretEncrypted { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>İlk tarihsel yükleme (backfill) tamamlandı mı?</summary>
    public bool BackfillCompleted { get; set; }

    /// <summary>Başarılı son artımlı çekme zamanı (idempotent ingestion için).</summary>
    public DateTimeOffset? LastSettlementSyncedAt { get; set; }
}
