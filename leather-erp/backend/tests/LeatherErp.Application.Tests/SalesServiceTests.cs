using LeatherErp.Application.Common;
using LeatherErp.Application.Services;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeatherErp.Application.Tests;

public class SalesServiceTests
{
    private static AppDbContext NewDb()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static (AppDbContext db, Guid productId) SeedWithStock(int onHand, decimal avgCost)
    {
        var db = NewDb();
        var product = new Product { Name = "Cüzdan" };
        db.Products.Add(product);
        db.FinishedGoods.Add(new FinishedGoodsInventory
        {
            Product = product, QuantityOnHand = onHand, AverageUnitCost = avgCost
        });
        db.SaveChanges();
        return (db, product.Id);
    }

    [Fact]
    public async Task Create_DecrementsStock_AndFreezesCogs()
    {
        var (db, productId) = SeedWithStock(10, 170m);
        var sale = await new SalesService(db).CreateAsync(productId, 3, 290m, "Müşteri A");

        Assert.Equal(170m, sale.UnitCost);          // COGS donduruldu
        Assert.Equal(870m, sale.Revenue);           // 3 × 290
        Assert.Equal(360m, sale.Profit);            // 3 × (290 − 170)

        var inv = await db.FinishedGoods.FirstAsync(f => f.ProductId == productId);
        Assert.Equal(7, inv.QuantityOnHand);        // 10 − 3
    }

    [Fact]
    public async Task Create_InsufficientStock_Throws()
    {
        var (db, productId) = SeedWithStock(2, 100m);
        await Assert.ThrowsAsync<InsufficientStockException>(
            () => new SalesService(db).CreateAsync(productId, 5, 150m));
    }

    [Fact]
    public async Task Create_NoInventory_Throws()
    {
        var db = NewDb();
        await Assert.ThrowsAsync<InsufficientStockException>(
            () => new SalesService(db).CreateAsync(Guid.NewGuid(), 1, 100m));
    }
}
