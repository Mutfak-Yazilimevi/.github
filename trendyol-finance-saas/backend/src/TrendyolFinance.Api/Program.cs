using System.Security.Claims;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Application.Cogs;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Infrastructure;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Accounting;
using TrendyolFinance.Integration.Trendyol;
using TrendyolFinance.Integration.Tuik;
using TrendyolFinance.Ingestion;

var builder = WebApplication.CreateBuilder(args);

// --- Servisler ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Çoklu kiracı: geçerli tenant (JWT claim'inden; dev'de X-Tenant-Id header'ı fallback).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, HttpCurrentTenant>();

// JWT kimlik doğrulama (IdP/Authority appsettings'ten).
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwt["Authority"];
        options.Audience = jwt["Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(jwt["Authority"]),
            ValidateAudience = !string.IsNullOrEmpty(jwt["Audience"]),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// Infrastructure: EF Core + PostgreSQL + şifreleme + kimlik çözücü
builder.Services.AddInfrastructure(builder.Configuration);

// Trendyol finans istemcisi
builder.Services.Configure<TrendyolOptions>(builder.Configuration.GetSection(TrendyolOptions.SectionName));
builder.Services.AddHttpClient<ITrendyolApiClient, TrendyolApiClient>((sp, http) =>
    http.BaseAddress = new Uri(TrendyolBaseUrl(builder.Configuration)));

// Trendyol katalog/komisyon istemcisi
builder.Services.AddHttpClient<ITrendyolCatalogClient, TrendyolCatalogClient>((sp, http) =>
    http.BaseAddress = new Uri(TrendyolBaseUrl(builder.Configuration)));

// TÜİK (TCMB EVDS) TÜFE sağlayıcısı
builder.Services.Configure<TuikOptions>(builder.Configuration.GetSection(TuikOptions.SectionName));
builder.Services.AddHttpClient<IInflationProvider, TuikEvdsInflationProvider>((sp, http) =>
    http.BaseAddress = new Uri(builder.Configuration[$"{TuikOptions.SectionName}:BaseUrl"]
                               ?? "https://evds2.tcmb.gov.tr/service/evds/"));

// Muhasebe sağlayıcısı (Paraşüt — iskelet)
builder.Services.Configure<ParasutOptions>(builder.Configuration.GetSection(ParasutOptions.SectionName));
builder.Services.AddHttpClient<IAccountingProvider, ParasutAccountingProvider>((sp, http) =>
    http.BaseAddress = new Uri(builder.Configuration[$"{ParasutOptions.SectionName}:BaseUrl"]
                               ?? "https://api.parasut.com/v4/"));

// Uygulama servisleri
builder.Services.AddScoped<ProfitCalculator>();
builder.Services.AddScoped<ReconciliationService>();
builder.Services.AddScoped<DeadStockService>();
builder.Services.AddScoped<RealPriceService>();
builder.Services.AddScoped<HealthScoreCalculator>();
builder.Services.AddScoped<ForecastService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<CogsCsvParser>();

// Ingestion
builder.Services.AddScoped<SettlementIngestionService>();
builder.Services.AddScoped<InflationImportService>();
builder.Services.AddScoped<CategoryCommissionSyncService>();
builder.Services.AddScoped<AccountingCostImportService>();
builder.Services.AddScoped<IngestionJobs>();

// Hangfire (arka plan + zamanlanmış işler)
builder.Services.AddHangfire(cfg => cfg
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/hangfire");

// --- Zamanlanmış işler ---
RecurringJob.AddOrUpdate<IngestionJobs>(
    "settlement-sync-hourly", j => j.SyncAllActiveAsync(CancellationToken.None), Cron.Hourly);
RecurringJob.AddOrUpdate<IngestionJobs>(
    "category-commission-daily", j => j.SyncCategoryCommissionsAsync(CancellationToken.None), Cron.Daily);
RecurringJob.AddOrUpdate<IngestionJobs>(
    "inflation-monthly", j => j.ImportInflationAsync(CancellationToken.None), Cron.Monthly);

app.Run();

static string TrendyolBaseUrl(IConfiguration config) =>
    config[$"{TrendyolOptions.SectionName}:BaseUrl"] ?? "https://apigw.trendyol.com/integration/";

/// <summary>Geçerli tenant'ı JWT "tenant_id" claim'inden; yoksa (dev) "X-Tenant-Id" header'ından çözer.</summary>
internal sealed class HttpCurrentTenant : ICurrentTenant
{
    public Guid? TenantId { get; }

    public HttpCurrentTenant(IHttpContextAccessor accessor)
    {
        var ctx = accessor.HttpContext;
        var claim = ctx?.User.FindFirstValue("tenant_id");
        var raw = claim ?? ctx?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(raw, out var id)) TenantId = id;
    }
}
