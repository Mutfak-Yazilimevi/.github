using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Infrastructure.Security;

namespace TrendyolFinance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Default")));

        services.AddDataProtection();
        services.AddScoped<ICredentialProtector, DataProtectionCredentialProtector>();
        services.AddScoped<ITrendyolCredentialResolver, TrendyolCredentialResolver>();

        return services;
    }
}
