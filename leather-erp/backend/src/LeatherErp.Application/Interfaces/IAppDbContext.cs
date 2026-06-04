using LeatherErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Application.Interfaces;

/// <summary>
/// Uygulama veritabanı sözleşmesi. Application katmanındaki servisler Infrastructure'a doğrudan
/// bağlı olmadan EF Core üzerinden çalışır.
/// </summary>
public interface IAppDbContext
{
    DbSet<Supplier> Suppliers { get; }
    DbSet<LeatherType> LeatherTypes { get; }
    DbSet<LeatherLot> LeatherLots { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductRecipe> Recipes { get; }
    DbSet<ProductionOrder> ProductionOrders { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<FinishedGoodsInventory> FinishedGoods { get; }
    DbSet<AppSettings> Settings { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
