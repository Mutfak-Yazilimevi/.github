using LeatherErp.Application.Common;
using LeatherErp.Application.Services;
using LeatherErp.Domain.Entities;
using LeatherErp.Domain.Enums;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeatherErp.Application.Tests;

public class ProductionServiceTests
{
    private static AppDbContext NewDb()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static (AppDbContext db, Guid productId, Guid lot1Id, Guid lot2Id) Seed()
    {
        var db = NewDb();
        var type = new LeatherType { Name = "Vaketa" };
        var product = new Product { Name = "Cüzdan" };
        var recipe = new ProductRecipe
        {
            Product = product, LeatherType = type,
            NetLeatherDm2 = 8m, WasteRate = 0.15m, LaborCost = 40m, OverheadCost = 15m
        };
        // İki lot, farklı maliyet → FIFO sırası test edilir.
        var lot1 = new LeatherLot
        {
            LeatherType = type, PurchaseDate = DateTime.UtcNow.AddDays(-10),
            QuantityDm2 = 50m, RemainingDm2 = 50m, UnitCostPerDm2 = 12.50m
        };
        var lot2 = new LeatherLot
        {
            LeatherType = type, PurchaseDate = DateTime.UtcNow.AddDays(-2),
            QuantityDm2 = 100m, RemainingDm2 = 100m, UnitCostPerDm2 = 13.80m
        };
        db.AddRange(type, product, recipe, lot1, lot2);
        db.SaveChanges();
        return (db, product.Id, lot1.Id, lot2.Id);
    }

    [Fact]
    public async Task Confirm_ConsumesOldestLotFirst_AndUpdatesFinishedGoods()
    {
        var (db, productId, lot1Id, lot2Id) = Seed();
        // 5 adet × 8 net × 1.15 fire = 46 dm² brüt gereksinim.
        var order = new ProductionOrder { ProductId = productId, Quantity = 5 };
        db.ProductionOrders.Add(order);
        db.SaveChanges();

        var sut = new ProductionService(db);
        var confirmed = await sut.ConfirmAsync(order.Id);

        Assert.Equal(ProductionStatus.Confirmed, confirmed.Status);
        Assert.Equal(46m, confirmed.TotalLeatherConsumedDm2);

        // En eski lot (50 dm²) 46'sını verir → 4 kalır; ikinci lot dokunulmaz.
        var lot1 = await db.LeatherLots.FindAsync(lot1Id);
        var lot2 = await db.LeatherLots.FindAsync(lot2Id);
        Assert.Equal(4m, lot1!.RemainingDm2);
        Assert.Equal(100m, lot2!.RemainingDm2);

        // Maliyet: malzeme 46×12.50=575, işçilik+genel (40+15)×5=275 → toplam 850, birim 170.
        Assert.Equal(170m, confirmed.UnitCostSnapshot);

        var inv = await db.FinishedGoods.FirstAsync(f => f.ProductId == productId);
        Assert.Equal(5, inv.QuantityOnHand);
        Assert.Equal(170m, inv.AverageUnitCost);

        // Stok çıkış hareketi kaydedildi.
        Assert.True(await db.StockMovements.AnyAsync(m => m.Direction == MovementDirection.Out && m.QuantityDm2 == 46m));
    }

    [Fact]
    public async Task Confirm_SpanningTwoLots_BlendsCost()
    {
        var (db, productId, _, _) = Seed();
        // 7 adet × 9.2 = 64.4 dm². İlk lot 50 (12.50), kalan 14.4 ikinci lottan (13.80).
        var order = new ProductionOrder { ProductId = productId, Quantity = 7 };
        db.ProductionOrders.Add(order);
        db.SaveChanges();

        var confirmed = await new ProductionService(db).ConfirmAsync(order.Id);

        var material = 50m * 12.50m + 14.4m * 13.80m;       // 625 + 198.72 = 823.72
        var total = material + (40m + 15m) * 7;             // + 385 = 1208.72
        Assert.Equal(Math.Round(total / 7, 4), Math.Round(confirmed.UnitCostSnapshot, 4));
    }

    [Fact]
    public async Task Confirm_InsufficientStock_Throws()
    {
        var (db, productId, _, _) = Seed();
        // 20 adet × 9.2 = 184 dm² > 150 mevcut.
        var order = new ProductionOrder { ProductId = productId, Quantity = 20 };
        db.ProductionOrders.Add(order);
        db.SaveChanges();

        await Assert.ThrowsAsync<InsufficientStockException>(() => new ProductionService(db).ConfirmAsync(order.Id));
    }

    [Fact]
    public async Task Confirm_AlreadyConfirmed_Throws()
    {
        var (db, productId, _, _) = Seed();
        var order = new ProductionOrder { ProductId = productId, Quantity = 1, Status = ProductionStatus.Confirmed };
        db.ProductionOrders.Add(order);
        db.SaveChanges();

        await Assert.ThrowsAsync<BusinessRuleException>(() => new ProductionService(db).ConfirmAsync(order.Id));
    }
}
