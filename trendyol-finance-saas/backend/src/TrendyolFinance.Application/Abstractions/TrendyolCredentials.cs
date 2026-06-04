namespace TrendyolFinance.Application.Abstractions;

/// <summary>
/// Tek bir mağaza için çözülmüş (decrypt edilmiş) Trendyol kimlik bilgileri.
/// Port türü Application'da yaşar; Integration adaptörleri bunu tüketir
/// (bağımlılık yönü: Integration → Application).
/// </summary>
public record TrendyolCredentials(long SellerId, string ApiKey, string ApiSecret);
