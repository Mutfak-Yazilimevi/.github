using Microsoft.AspNetCore.DataProtection;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Sellers;

namespace TrendyolFinance.Infrastructure.Security;

/// <summary>ASP.NET Core Data Protection ile sır şifreleme. Üretimde anahtarlar KMS/kalıcı depoya alınmalı.</summary>
public class DataProtectionCredentialProtector : ICredentialProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionCredentialProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("TrendyolFinance.Credentials.v1");
    }

    public string Protect(string plaintext) => _protector.Protect(plaintext);
    public string Unprotect(string ciphertext) => _protector.Unprotect(ciphertext);
}

/// <summary>Şifreli mağaza kimliklerini çözüp Trendyol istemcisine uygun forma getirir.</summary>
public class TrendyolCredentialResolver : ITrendyolCredentialResolver
{
    private readonly ICredentialProtector _protector;

    public TrendyolCredentialResolver(ICredentialProtector protector) => _protector = protector;

    public TrendyolCredentials Resolve(SellerAccount account) => new(
        account.TrendyolSellerId,
        _protector.Unprotect(account.ApiKeyEncrypted),
        _protector.Unprotect(account.ApiSecretEncrypted));
}
