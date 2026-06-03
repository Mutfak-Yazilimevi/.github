namespace TrendyolFinance.Application.Abstractions;

/// <summary>Trendyol API sırlarını şifreler/çözer (Infrastructure'da Data Protection ile uygulanır).</summary>
public interface ICredentialProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}
