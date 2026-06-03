using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Domain.Catalog;
using TrendyolFinance.Domain.Common;
using TrendyolFinance.Domain.Finance;
using TrendyolFinance.Domain.Inflation;
using TrendyolFinance.Domain.Sellers;
using TrendyolFinance.Domain.Tenancy;

namespace TrendyolFinance.Infrastructure.Persistence;

/// <summary>Geçerli isteğin tenant kimliğini sağlar (API katmanında JWT'den çözülür).</summary>
public interface ICurrentTenant
{
    Guid? TenantId { get; }
}

public class AppDbContext : DbContext
{
    private readonly ICurrentTenant _currentTenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenant currentTenant)
        : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<SellerAccount> SellerAccounts => Set<SellerAccount>();
    public DbSet<SettlementTransaction> SettlementTransactions => Set<SettlementTransaction>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCost> ProductCosts => Set<ProductCost>();
    public DbSet<PriceSnapshot> PriceSnapshots => Set<PriceSnapshot>();
    public DbSet<InflationIndex> InflationIndices => Set<InflationIndex>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Idempotent ingestion: aynı Trendyol kalemi tek kez.
        b.Entity<SettlementTransaction>()
            .HasIndex(x => new { x.SellerAccountId, x.TrendyolTransactionId })
            .IsUnique();

        b.Entity<Product>()
            .HasIndex(x => new { x.SellerAccountId, x.Barcode })
            .IsUnique();

        b.Entity<InflationIndex>().HasKey(x => x.YearMonth);

        // Çoklu kiracı izolasyonu: ITenantScoped varlıklarına global filtre.
        ApplyTenantFilter<AppUser>(b);
        ApplyTenantFilter<SellerAccount>(b);
        ApplyTenantFilter<SettlementTransaction>(b);
        ApplyTenantFilter<Product>(b);
        ApplyTenantFilter<ProductCost>(b);
        ApplyTenantFilter<PriceSnapshot>(b);
    }

    private void ApplyTenantFilter<T>(ModelBuilder b) where T : class, ITenantScoped =>
        b.Entity<T>().HasQueryFilter(e => _currentTenant.TenantId == null || e.TenantId == _currentTenant.TenantId);

    public override int SaveChanges()
    {
        StampTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        StampTimestamps();
        return base.SaveChangesAsync(ct);
    }

    private void StampTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
