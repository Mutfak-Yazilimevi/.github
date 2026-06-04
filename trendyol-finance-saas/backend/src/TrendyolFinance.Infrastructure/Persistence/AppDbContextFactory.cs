using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrendyolFinance.Infrastructure.Persistence;

/// <summary>
/// `dotnet ef migrations add ...` için design-time DbContext üretici.
/// AppDbContext ctor'u ICurrentTenant beklediğinden burada boş bir tenant sağlanır.
/// Bağlantı dizesi env: TRENDYOL_FINANCE_DB (yoksa localhost varsayılanı).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("TRENDYOL_FINANCE_DB")
            ?? "Host=localhost;Port=5432;Database=trendyolfinance;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new AppDbContext(options, new NullTenant());
    }

    private sealed class NullTenant : ICurrentTenant
    {
        public Guid? TenantId => null;
    }
}
