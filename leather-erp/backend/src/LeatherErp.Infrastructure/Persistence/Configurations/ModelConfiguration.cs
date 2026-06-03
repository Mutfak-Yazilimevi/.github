using LeatherErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeatherErp.Infrastructure.Persistence.Configurations;

/// <summary>Para/miktar alanları için ondalık hassasiyet ve ilişki/kısıt yapılandırmaları.</summary>
public class SupplierConfig : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
    }
}

public class LeatherTypeConfig : IEntityTypeConfiguration<LeatherType>
{
    public void Configure(EntityTypeBuilder<LeatherType> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.ThicknessMm).HasPrecision(8, 2);
        b.Property(x => x.LowStockThresholdDm2).HasPrecision(18, 4);
        b.HasIndex(x => x.Name);
    }
}

public class LeatherLotConfig : IEntityTypeConfiguration<LeatherLot>
{
    public void Configure(EntityTypeBuilder<LeatherLot> b)
    {
        b.Property(x => x.QuantityDm2).HasPrecision(18, 4);
        b.Property(x => x.RemainingDm2).HasPrecision(18, 4);
        b.Property(x => x.UnitCostPerDm2).HasPrecision(18, 4);
        b.HasOne(x => x.LeatherType).WithMany(t => t.Lots)
            .HasForeignKey(x => x.LeatherTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Supplier).WithMany(s => s.Lots)
            .HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => new { x.LeatherTypeId, x.PurchaseDate });
    }
}

public class StockMovementConfig : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> b)
    {
        b.Property(x => x.QuantityDm2).HasPrecision(18, 4);
        b.HasOne(x => x.LeatherLot).WithMany(l => l.Movements)
            .HasForeignKey(x => x.LeatherLotId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.ProductionOrderId);
    }
}

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Sku).HasMaxLength(64);
        b.HasIndex(x => x.Sku).IsUnique().HasFilter("\"Sku\" IS NOT NULL");
        b.HasOne(x => x.Recipe).WithOne(r => r.Product)
            .HasForeignKey<ProductRecipe>(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Inventory).WithOne(i => i.Product)
            .HasForeignKey<FinishedGoodsInventory>(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductRecipeConfig : IEntityTypeConfiguration<ProductRecipe>
{
    public void Configure(EntityTypeBuilder<ProductRecipe> b)
    {
        b.Property(x => x.NetLeatherDm2).HasPrecision(18, 4);
        b.Property(x => x.WasteRate).HasPrecision(6, 4);
        b.Property(x => x.LaborCost).HasPrecision(18, 4);
        b.Property(x => x.OverheadCost).HasPrecision(18, 4);
        b.HasOne(x => x.LeatherType).WithMany()
            .HasForeignKey(x => x.LeatherTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductionOrderConfig : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> b)
    {
        b.Property(x => x.UnitCostSnapshot).HasPrecision(18, 4);
        b.Property(x => x.TotalLeatherConsumedDm2).HasPrecision(18, 4);
        b.HasOne(x => x.Product).WithMany()
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FinishedGoodsConfig : IEntityTypeConfiguration<FinishedGoodsInventory>
{
    public void Configure(EntityTypeBuilder<FinishedGoodsInventory> b)
    {
        b.Property(x => x.AverageUnitCost).HasPrecision(18, 4);
        b.Ignore(x => x.TotalValue);
        b.HasIndex(x => x.ProductId).IsUnique();
    }
}

public class AppSettingsConfig : IEntityTypeConfiguration<AppSettings>
{
    public void Configure(EntityTypeBuilder<AppSettings> b)
    {
        b.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(8);
        b.Property(x => x.VatRate).HasPrecision(6, 4);
        b.Property(x => x.DefaultWasteRate).HasPrecision(6, 4);
        b.Property(x => x.DefaultProfitMargin).HasPrecision(6, 4);
    }
}

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.Property(x => x.Username).IsRequired().HasMaxLength(64);
        b.HasIndex(x => x.Username).IsUnique();
    }
}
