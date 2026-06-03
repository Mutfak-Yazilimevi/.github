using LeatherErp.Domain.Common;
using LeatherErp.Domain.Enums;

namespace LeatherErp.Domain.Entities;

/// <summary>Uygulama geneli ayarlar (tek satır). Birim, para birimi, KDV ve varsayılan oranlar.</summary>
public class AppSettings : BaseEntity
{
    /// <summary>Arayüzde gösterilecek/girilecek deri ölçü birimi.</summary>
    public UnitOfMeasure DisplayUnit { get; set; } = UnitOfMeasure.Dm2;

    /// <summary>Para birimi kodu (örn. "TRY", "USD", "EUR").</summary>
    public string CurrencyCode { get; set; } = "TRY";

    /// <summary>KDV oranı (0–1 arası; örn. 0.20 = %20).</summary>
    public decimal VatRate { get; set; } = 0.20m;

    /// <summary>Yeni reçeteler için varsayılan fire oranı (0–1).</summary>
    public decimal DefaultWasteRate { get; set; } = 0.15m;

    /// <summary>Fiyatlandırmada varsayılan kâr marjı (0–1; maliyet üzerine eklenir).</summary>
    public decimal DefaultProfitMargin { get; set; } = 0.40m;
}
