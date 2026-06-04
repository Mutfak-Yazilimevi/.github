using LeatherErp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeatherErp.Application;

/// <summary>Application katmanı servislerini DI konteynerine kaydeder.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<CostCalculationService>();
        services.AddSingleton<PricingService>();
        services.AddScoped<ProductionService>();
        services.AddScoped<StockService>();
        services.AddScoped<ProductCostingService>();
        services.AddScoped<ReportService>();
        return services;
    }
}
