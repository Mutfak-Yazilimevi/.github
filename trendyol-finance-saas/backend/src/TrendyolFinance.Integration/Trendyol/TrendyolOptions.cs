namespace TrendyolFinance.Integration.Trendyol;

/// <summary>Trendyol API genel ayarları (appsettings: "Trendyol").</summary>
public class TrendyolOptions
{
    public const string SectionName = "Trendyol";

    /// <summary>API taban adresi. NOT: resmî dokümandan teyit edilecek.</summary>
    public string BaseUrl { get; set; } = "https://apigw.trendyol.com/integration/";

    /// <summary>Settlement sorgularında izin verilen azami gün penceresi (tipik ~15).</summary>
    public int MaxSettlementWindowDays { get; set; } = 15;

    /// <summary>Sayfa boyutu.</summary>
    public int PageSize { get; set; } = 500;
}
