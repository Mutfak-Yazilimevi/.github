using LeatherErp.Application.Common;
using LeatherErp.Application.Interfaces;
using LeatherErp.Domain.Entities;
using LeatherErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Services;

/// <summary>
/// Üretim emri onayı: gerekli brüt deriyi hesaplar, deri lotlarından FIFO (en eski önce) düşer,
/// mamul stoğu ağırlıklı ortalama maliyetle günceller ve stok hareketlerini kaydeder.
/// Tüm değişiklikler tek <c>SaveChanges</c> ile atomik olarak işlenir.
/// </summary>
public class ProductionService
{
    private readonly IAppDbContext _db;

    public ProductionService(IAppDbContext db) => _db = db;

    /// <summary>Taslak bir üretim emrini onaylar ve stokları işler.</summary>
    public async Task<ProductionOrder> ConfirmAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _db.ProductionOrders
            .Include(o => o.Product).ThenInclude(p => p.Recipe)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new NotFoundException($"Üretim emri bulunamadı: {orderId}");

        if (order.Status != ProductionStatus.Draft)
            throw new BusinessRuleException("Yalnızca taslak durumundaki üretim emirleri onaylanabilir.");
        if (order.Quantity <= 0)
            throw new BusinessRuleException("Üretim adedi sıfırdan büyük olmalıdır.");

        var recipe = order.Product.Recipe
            ?? throw new BusinessRuleException($"'{order.Product.Name}' ürününün reçetesi tanımlı değil.");

        var grossPerUnit = recipe.NetLeatherDm2 * (1 + recipe.WasteRate);
        var totalNeeded = grossPerUnit * order.Quantity;

        // FIFO: ilgili deri tipinin lotlarını en eski alımdan başlayarak getir.
        var lots = await _db.LeatherLots
            .Where(l => l.LeatherTypeId == recipe.LeatherTypeId && l.RemainingDm2 > 0)
            .OrderBy(l => l.PurchaseDate).ThenBy(l => l.CreatedAt)
            .ToListAsync(ct);

        var available = lots.Sum(l => l.RemainingDm2);
        if (available < totalNeeded)
            throw new InsufficientStockException(
                $"Yetersiz deri stoğu. Gerekli: {totalNeeded:0.##} dm², mevcut: {available:0.##} dm².");

        decimal remainingToConsume = totalNeeded;
        decimal realizedMaterialCost = 0m;

        foreach (var lot in lots)
        {
            if (remainingToConsume <= 0) break;

            var take = Math.Min(lot.RemainingDm2, remainingToConsume);
            lot.RemainingDm2 -= take;
            lot.UpdatedAt = DateTime.UtcNow;
            remainingToConsume -= take;
            realizedMaterialCost += take * lot.UnitCostPerDm2;

            _db.StockMovements.Add(new StockMovement
            {
                LeatherLotId = lot.Id,
                Direction = MovementDirection.Out,
                QuantityDm2 = take,
                Reason = $"Üretim emri {order.Id:N} — {order.Product.Name}",
                ProductionOrderId = order.Id
            });
        }

        var totalCost = realizedMaterialCost + (recipe.LaborCost + recipe.OverheadCost) * order.Quantity;
        var unitCost = totalCost / order.Quantity;

        order.Status = ProductionStatus.Confirmed;
        order.UnitCostSnapshot = unitCost;
        order.TotalLeatherConsumedDm2 = totalNeeded;
        order.UpdatedAt = DateTime.UtcNow;

        // Mamul stoğu ağırlıklı ortalama maliyetle güncelle.
        var inv = await _db.FinishedGoods.FirstOrDefaultAsync(f => f.ProductId == order.ProductId, ct);
        if (inv is null)
        {
            inv = new FinishedGoodsInventory
            {
                ProductId = order.ProductId,
                QuantityOnHand = order.Quantity,
                AverageUnitCost = unitCost
            };
            _db.FinishedGoods.Add(inv);
        }
        else
        {
            var totalQty = inv.QuantityOnHand + order.Quantity;
            inv.AverageUnitCost = ((inv.QuantityOnHand * inv.AverageUnitCost) + totalCost) / totalQty;
            inv.QuantityOnHand = totalQty;
            inv.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return order;
    }
}
