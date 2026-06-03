namespace TrendyolFinance.Domain.Finance;

/// <summary>
/// Kategori bazında Trendyol komisyon oranı (mutabakatta beklenen komisyon hesabı için).
/// Trendyol kategori servisinden beslenir; tenant'tan bağımsız referans verisidir.
/// </summary>
public class CategoryCommission
{
    public int TrendyolCategoryId { get; set; }
    public string? CategoryName { get; set; }

    /// <summary>Komisyon oranı (% — örn. 12.5).</summary>
    public decimal CommissionRate { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
