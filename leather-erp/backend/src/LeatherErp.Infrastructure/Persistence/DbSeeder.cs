using LeatherErp.Application.Common;
using LeatherErp.Domain.Entities;
using LeatherErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Infrastructure.Persistence;

/// <summary>Boş veritabanını başlangıç verisiyle doldurur (idempotent): ayarlar, admin, örnek deri ve ürün.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Settings.AnyAsync())
            db.Settings.Add(new AppSettings());

        if (!await db.Users.AnyAsync())
            db.Users.Add(new User
            {
                Username = "admin",
                FullName = "Yönetici",
                Role = UserRole.Admin,
                PasswordHash = PasswordHasher.Hash("admin123")
            });

        if (!await db.LeatherTypes.AnyAsync())
        {
            var supplier = new Supplier { Name = "Örnek Deri Tedarik A.Ş.", Phone = "+90 212 000 0000" };

            var vaketa = new LeatherType
            {
                Name = "Vaketa - Kahverengi - 1.8mm",
                Kind = "Vaketa", Color = "Kahverengi", ThicknessMm = 1.8m,
                LowStockThresholdDm2 = 200m
            };
            var nubuk = new LeatherType
            {
                Name = "Nubuk - Siyah - 1.4mm",
                Kind = "Nubuk", Color = "Siyah", ThicknessMm = 1.4m,
                LowStockThresholdDm2 = 150m
            };

            db.Suppliers.Add(supplier);
            db.LeatherTypes.AddRange(vaketa, nubuk);

            // Örnek lotlar (farklı birim maliyet → FIFO etkisini gösterir).
            db.LeatherLots.AddRange(
                new LeatherLot
                {
                    LeatherType = vaketa, Supplier = supplier, LotNumber = "V-2026-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-30),
                    QuantityDm2 = 500m, RemainingDm2 = 500m, UnitCostPerDm2 = 12.50m
                },
                new LeatherLot
                {
                    LeatherType = vaketa, Supplier = supplier, LotNumber = "V-2026-002",
                    PurchaseDate = DateTime.UtcNow.AddDays(-5),
                    QuantityDm2 = 300m, RemainingDm2 = 300m, UnitCostPerDm2 = 13.80m
                },
                new LeatherLot
                {
                    LeatherType = nubuk, Supplier = supplier, LotNumber = "N-2026-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-10),
                    QuantityDm2 = 400m, RemainingDm2 = 400m, UnitCostPerDm2 = 15.00m
                });

            // Örnek ürün + reçete: el yapımı cüzdan.
            var product = new Product { Name = "El Yapımı Deri Cüzdan", Sku = "CZ-001", Category = "Cüzdan" };
            db.Products.Add(product);
            db.Recipes.Add(new ProductRecipe
            {
                Product = product, LeatherType = vaketa,
                NetLeatherDm2 = 8m, WasteRate = 0.15m,
                LaborCost = 40m, OverheadCost = 15m
            });
        }

        await db.SaveChangesAsync();
    }
}
