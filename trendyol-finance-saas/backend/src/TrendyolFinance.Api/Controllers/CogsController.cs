using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Cogs;
using TrendyolFinance.Domain.Catalog;
using TrendyolFinance.Infrastructure.Persistence;

namespace TrendyolFinance.Api.Controllers;

public record CogsManualRequest(
    string Barcode, decimal PurchasePrice, decimal VatRate,
    decimal PackagingCost, decimal OtherCost, DateOnly EffectiveFrom);

/// <summary>COGS (alış maliyeti) girişi: manuel + Excel/CSV import + geçmiş sorgu.</summary>
[ApiController]
[Route("api/[controller]")]
public class CogsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CogsCsvParser _csv;

    public CogsController(AppDbContext db, CogsCsvParser csv)
    {
        _db = db;
        _csv = csv;
    }

    /// <summary>Tek ürün için manuel maliyet girişi (effective-dated).</summary>
    [HttpPost]
    public async Task<IActionResult> AddManual([FromBody] CogsManualRequest req, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Barcode == req.Barcode, ct);
        if (product is null) return NotFound($"Ürün bulunamadı: {req.Barcode}");

        var row = new CogsImportRow(req.Barcode, req.PurchasePrice, req.VatRate,
            req.PackagingCost, req.OtherCost, req.EffectiveFrom);
        await ApplyRowAsync(product, row, CostSource.Manual, ct);
        await _db.SaveChangesAsync(ct);
        return Ok();
    }

    /// <summary>Excel (.xlsx) veya CSV ile toplu maliyet içe aktarımı.</summary>
    [HttpPost("import")]
    public async Task<ActionResult<CogsImportResult>> Import(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) return BadRequest("Dosya boş.");

        CogsImportResult parsed = file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
            ? ParseExcel(file)
            : _csv.Parse(await ReadLinesAsync(file, ct));

        if (parsed.HasErrors && parsed.ValidRows.Count == 0)
            return BadRequest(parsed);

        // Geçerli satırları uygula (eşleşmeyen barkodlar hata listesine eklenir).
        var errors = parsed.Errors.ToList();
        var products = await _db.Products
            .Where(p => parsed.ValidRows.Select(r => r.Barcode).Contains(p.Barcode))
            .ToDictionaryAsync(p => p.Barcode, ct);

        var applied = 0;
        foreach (var row in parsed.ValidRows)
        {
            if (!products.TryGetValue(row.Barcode, out var product))
            {
                errors.Add($"Ürün bulunamadı: {row.Barcode}");
                continue;
            }
            await ApplyRowAsync(product, row, CostSource.ExcelImport, ct);
            applied++;
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new CogsImportResult(parsed.ValidRows, errors));
    }

    /// <summary>Ürünün maliyet geçmişi (effective-dated).</summary>
    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<IEnumerable<ProductCost>>> History(Guid productId, CancellationToken ct) =>
        Ok(await _db.ProductCosts.Where(c => c.ProductId == productId)
            .OrderByDescending(c => c.EffectiveFrom).ToListAsync(ct));

    // --- yardımcılar ---

    /// <summary>Yeni maliyeti ekler ve önceki açık maliyet kaydını kapatır (effective-dating).</summary>
    private async Task ApplyRowAsync(Product product, CogsImportRow row, CostSource source, CancellationToken ct)
    {
        var previousOpen = await _db.ProductCosts
            .Where(c => c.ProductId == product.Id && c.EffectiveTo == null && c.EffectiveFrom < row.EffectiveFrom)
            .OrderByDescending(c => c.EffectiveFrom)
            .FirstOrDefaultAsync(ct);

        if (previousOpen is not null)
            previousOpen.EffectiveTo = row.EffectiveFrom.AddDays(-1);

        _db.ProductCosts.Add(CogsCsvParser.ToCost(row, product.TenantId, product.Id, source));
    }

    private static async Task<List<string>> ReadLinesAsync(IFormFile file, CancellationToken ct)
    {
        var lines = new List<string>();
        using var reader = new StreamReader(file.OpenReadStream());
        while (await reader.ReadLineAsync(ct) is { } line)
            lines.Add(line);
        return lines;
    }

    private CogsImportResult ParseExcel(IFormFile file)
    {
        using var wb = new XLWorkbook(file.OpenReadStream());
        var ws = wb.Worksheets.First();
        // Excel'i CSV satırlarına indirip aynı ayrıştırıcıdan geçir (tek doğruluk kaynağı).
        var lines = ws.RowsUsed().Select(r =>
            string.Join(",", Enumerable.Range(1, 6).Select(c => r.Cell(c).GetString())));
        return _csv.Parse(lines);
    }
}
