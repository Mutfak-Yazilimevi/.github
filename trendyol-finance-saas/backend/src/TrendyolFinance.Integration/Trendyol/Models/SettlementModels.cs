namespace TrendyolFinance.Integration.Trendyol.Models;

/// <summary>Trendyol settlement uç noktası için sorgu parametreleri.</summary>
public record SettlementQuery(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? TransactionType = null,
    int Page = 0);

/// <summary>Settlement yanıtının tek bir kalemi. Alan adları resmî dokümandan teyit edilecek.</summary>
public class SettlementItemDto
{
    public string Id { get; set; } = default!;
    public string TransactionType { get; set; } = default!;
    public string? OrderNumber { get; set; }
    public string? Barcode { get; set; }
    public long TransactionDate { get; set; }    // epoch ms
    public decimal Credit { get; set; }
    public decimal Debt { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal SellerRevenue { get; set; }
    public string Currency { get; set; } = "TRY";
}

/// <summary>Sayfalı settlement yanıtı.</summary>
public class SettlementResponse
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalPages { get; set; }
    public long TotalElements { get; set; }
    public List<SettlementItemDto> Content { get; set; } = new();
}
