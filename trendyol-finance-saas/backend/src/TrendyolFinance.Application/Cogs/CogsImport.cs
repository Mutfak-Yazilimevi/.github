using System.Globalization;
using TrendyolFinance.Domain.Catalog;

namespace TrendyolFinance.Application.Cogs;

/// <summary>İçe aktarılan tek bir COGS satırı (Excel/CSV).</summary>
public record CogsImportRow(
    string Barcode,
    decimal PurchasePrice,
    decimal VatRate,
    decimal PackagingCost,
    decimal OtherCost,
    DateOnly EffectiveFrom);

public record CogsImportResult(
    IReadOnlyList<CogsImportRow> ValidRows,
    IReadOnlyList<string> Errors)
{
    public bool HasErrors => Errors.Count > 0;
}

/// <summary>
/// COGS CSV ayrıştırıcı. Beklenen başlık:
/// barcode,purchasePrice,vatRate,packagingCost,otherCost,effectiveFrom
/// Sayılarda hem "." hem "," ondalık kabul edilir; tarih ISO (yyyy-MM-dd).
/// Excel (.xlsx) controller'da ClosedXML ile aynı satır yapısına dönüştürülür.
/// </summary>
public class CogsCsvParser
{
    private static readonly string[] Expected =
        { "barcode", "purchaseprice", "vatrate", "packagingcost", "othercost", "effectivefrom" };

    public CogsImportResult Parse(IEnumerable<string> lines)
    {
        var valid = new List<CogsImportRow>();
        var errors = new List<string>();

        using var e = lines.GetEnumerator();
        if (!e.MoveNext())
            return new CogsImportResult(valid, new[] { "Dosya boş." });

        var header = e.Current.Split(',').Select(h => h.Trim().ToLowerInvariant()).ToArray();
        if (!Expected.SequenceEqual(header))
            errors.Add($"Başlık beklenenle uyuşmuyor. Beklenen: {string.Join(",", Expected)}");

        var lineNo = 1;
        while (e.MoveNext())
        {
            lineNo++;
            var line = e.Current;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = line.Split(',');
            if (cols.Length < 6)
            {
                errors.Add($"Satır {lineNo}: 6 sütun bekleniyor, {cols.Length} bulundu.");
                continue;
            }

            try
            {
                valid.Add(new CogsImportRow(
                    cols[0].Trim(),
                    ParseDecimal(cols[1]),
                    ParseDecimal(cols[2]),
                    ParseDecimal(cols[3]),
                    ParseDecimal(cols[4]),
                    DateOnly.ParseExact(cols[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {
                errors.Add($"Satır {lineNo}: ayrıştırılamadı ({ex.Message}).");
            }
        }

        return new CogsImportResult(valid, errors);
    }

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);

    /// <summary>Bir import satırını yeni bir ProductCost'a çevirir (effective-dated; önceki kayıt kapatılmalı).</summary>
    public static ProductCost ToCost(CogsImportRow row, Guid tenantId, Guid productId, CostSource source) => new()
    {
        TenantId = tenantId,
        ProductId = productId,
        EffectiveFrom = row.EffectiveFrom,
        PurchasePrice = row.PurchasePrice,
        VatRate = row.VatRate,
        PackagingCost = row.PackagingCost,
        OtherCost = row.OtherCost,
        Source = source
    };
}
