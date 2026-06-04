using LeatherErp.Application.Interfaces;
using LeatherErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Infrastructure.Persistence;

/// <summary>EF Core veritabanı bağlamı. PostgreSQL üzerinde çalışır.</summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<LeatherType> LeatherTypes => Set<LeatherType>();
    public DbSet<LeatherLot> LeatherLots => Set<LeatherLot>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductRecipe> Recipes => Set<ProductRecipe>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<FinishedGoodsInventory> FinishedGoods => Set<FinishedGoodsInventory>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
