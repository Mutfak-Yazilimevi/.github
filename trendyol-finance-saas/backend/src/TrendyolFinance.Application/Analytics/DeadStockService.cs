using TrendyolFinance.Domain.Catalog;

namespace TrendyolFinance.Application.Analytics;

public record DeadStockItem(
    Guid ProductId,
    string Barcode,
    string Title,
    int Stock,
    decimal? UnitCost,
    decimal? TiedCapital,
    int DaysSinceLastSale,
    DateTimeOffset? LastSoldAt);

/// <summary>Uzun süredir satılmayan ürünleri ve bağlı sermayeyi tespit eder (F5).</summary>
public class DeadStockService
{
    /// <param name="costResolver">Ürünün güncel birim maliyetini döndürür (yoksa null).</param>
    public IReadOnlyList<DeadStockItem> Find(
        IEnumerable<Product> products,
        DateTimeOffset asOf,
        int thresholdDays,
        Func<Guid, decimal?> costResolver)
    {
        var result = new List<DeadStockItem>();

        foreach (var p in products)
        {
            if (p.CurrentStock <= 0) continue;

            var days = p.LastSoldAt is null
                ? int.MaxValue
                : (int)(asOf - p.LastSoldAt.Value).TotalDays;

            if (days < thresholdDays) continue;

            var unitCost = costResolver(p.Id);
            var tied = unitCost is null ? (decimal?)null : unitCost.Value * p.CurrentStock;

            result.Add(new DeadStockItem(
                p.Id, p.Barcode, p.Title, p.CurrentStock, unitCost, tied,
                days == int.MaxValue ? -1 : days, p.LastSoldAt));
        }

        // Bağlı sermayesi en yüksek olan önce.
        return result.OrderByDescending(x => x.TiedCapital ?? 0).ToList();
    }
}
