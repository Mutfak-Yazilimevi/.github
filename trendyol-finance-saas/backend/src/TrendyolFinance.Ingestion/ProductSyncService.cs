using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Catalog;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Trendyol;

namespace TrendyolFinance.Ingestion;

/// <summary>
/// Mağaza ürünlerini Trendyol'dan çekip Product tablosuna upsert eder; günlük PriceSnapshot kaydeder (F4)
/// ve son satış tarihini settlement'lardan günceller (F5).
/// </summary>
public class ProductSyncService
{
    private readonly AppDbContext _db;
    private readonly ITrendyolCatalogClient _catalog;
    private readonly ITrendyolCredentialResolver _credentials;
    private readonly ILogger<ProductSyncService> _logger;

    public ProductSyncService(
        AppDbContext db, ITrendyolCatalogClient catalog,
        ITrendyolCredentialResolver credentials, ILogger<ProductSyncService> logger)
    {
        _db = db;
        _catalog = catalog;
        _credentials = credentials;
        _logger = logger;
    }

    public async Task SyncAsync(Guid sellerAccountId, CancellationToken ct = default)
    {
        var account = await _db.SellerAccounts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == sellerAccountId, ct)
            ?? throw new InvalidOperationException($"SellerAccount {sellerAccountId} bulunamadı.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await _db.Products.IgnoreQueryFilters()
            .Where(p => p.SellerAccountId == sellerAccountId)
            .ToDictionaryAsync(p => p.Barcode, ct);

        // Bugün zaten snapshot alınmış ürünler (gün içi tekrar çalışmaya karşı).
        var snappedToday = (await _db.PriceSnapshots.IgnoreQueryFilters()
            .Where(s => s.TenantId == account.TenantId && s.CapturedOn == today)
            .Select(s => s.ProductId)
            .ToListAsync(ct)).ToHashSet();

        var count = 0;
        await foreach (var dto in _catalog.StreamProductsAsync(_credentials.Resolve(account), ct))
        {
            if (!existing.TryGetValue(dto.Barcode, out var product))
            {
                product = new Product
                {
                    TenantId = account.TenantId,
                    SellerAccountId = sellerAccountId,
                    Barcode = dto.Barcode,
                    Title = dto.Title
                };
                _db.Products.Add(product);
                existing[dto.Barcode] = product;
            }

            product.StockCode = dto.StockCode;
            product.Title = dto.Title;
            product.TrendyolCategoryId = dto.CategoryId;
            product.CategoryName = dto.CategoryName;
            product.Brand = dto.Brand;
            product.CurrentListPrice = dto.ListPrice;
            product.CurrentSalePrice = dto.SalePrice;
            product.CurrentStock = dto.Quantity;

            if (!snappedToday.Contains(product.Id))
            {
                _db.PriceSnapshots.Add(new PriceSnapshot
                {
                    TenantId = account.TenantId,
                    ProductId = product.Id,
                    CapturedOn = today,
                    ListPrice = dto.ListPrice,
                    SalePrice = dto.SalePrice
                });
            }
            count++;
        }

        await _db.SaveChangesAsync(ct);
        await UpdateLastSoldAsync(account.TenantId, sellerAccountId, ct);
        _logger.LogInformation("Ürün senkronu tamam: {Account} — {Count} ürün", sellerAccountId, count);
    }

    /// <summary>Her ürünün son satış tarihini settlement satışlarından günceller (F5 ölü stok için).</summary>
    private async Task UpdateLastSoldAsync(Guid tenantId, Guid sellerAccountId, CancellationToken ct)
    {
        var lastSold = await _db.SettlementTransactions.IgnoreQueryFilters()
            .Where(t => t.SellerAccountId == sellerAccountId
                && t.TransactionType == SettlementTransactionType.Sale && t.Barcode != null)
            .GroupBy(t => t.Barcode!)
            .Select(g => new { Barcode = g.Key, Last = g.Max(x => x.TransactionDate) })
            .ToDictionaryAsync(x => x.Barcode, x => x.Last, ct);

        if (lastSold.Count == 0) return;

        var products = await _db.Products.IgnoreQueryFilters()
            .Where(p => p.SellerAccountId == sellerAccountId)
            .ToListAsync(ct);

        foreach (var p in products)
            if (lastSold.TryGetValue(p.Barcode, out var last))
                p.LastSoldAt = last;

        await _db.SaveChangesAsync(ct);
    }
}
