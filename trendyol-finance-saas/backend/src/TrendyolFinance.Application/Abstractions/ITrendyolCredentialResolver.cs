using TrendyolFinance.Domain.Sellers;
using TrendyolFinance.Integration.Trendyol;

namespace TrendyolFinance.Application.Abstractions;

/// <summary>Bir mağaza hesabının şifreli kimlik bilgilerini çözüp Trendyol istemcisine uygun forma getirir.</summary>
public interface ITrendyolCredentialResolver
{
    TrendyolCredentials Resolve(SellerAccount account);
}
