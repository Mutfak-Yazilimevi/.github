using LeatherErp.Application.Common;
using LeatherErp.Application.Interfaces;
using LeatherErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Services;

/// <summary>Satış kaydı oluşturur: mamul stoğundan düşer ve satış anındaki maliyeti (COGS) dondurur.</summary>
public class SalesService
{
    private readonly IAppDbContext _db;

    public SalesService(IAppDbContext db) => _db = db;

    public async Task<SalesOrder> CreateAsync(
        Guid productId, int quantity, decimal unitPrice,
        string? customerName = null, string? notes = null, DateTime? saleDate = null,
        CancellationToken ct = default)
    {
        if (quantity <= 0) throw new BusinessRuleException("Satış adedi sıfırdan büyük olmalıdır.");
        if (unitPrice < 0) throw new BusinessRuleException("Birim satış fiyatı negatif olamaz.");

        var inv = await _db.FinishedGoods.FirstOrDefaultAsync(f => f.ProductId == productId, ct)
            ?? throw new InsufficientStockException("Bu ürün için mamul stoğu yok.");
        if (inv.QuantityOnHand < quantity)
            throw new InsufficientStockException(
                $"Yetersiz mamul stoğu. İstenen: {quantity} adet, mevcut: {inv.QuantityOnHand} adet.");

        var sale = new SalesOrder
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            UnitCost = inv.AverageUnitCost,
            CustomerName = customerName,
            Notes = notes,
            SaleDate = saleDate ?? DateTime.UtcNow
        };

        inv.QuantityOnHand -= quantity;
        inv.UpdatedAt = DateTime.UtcNow;
        _db.SalesOrders.Add(sale);

        await _db.SaveChangesAsync(ct);
        return sale;
    }
}
