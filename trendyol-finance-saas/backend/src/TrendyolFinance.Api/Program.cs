using Hangfire;
using Hangfire.PostgreSql;
using TrendyolFinance.Application.Analytics;
using TrendyolFinance.Application.Cogs;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Infrastructure;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Trendyol;
using TrendyolFinance.Ingestion;

var builder = WebApplication.CreateBuilder(args);

// --- Servisler ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Çoklu kiracı: geçerli tenant (MVP'de header'dan; üretimde JWT claim'inden).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, HttpCurrentTenant>();

// Infrastructure: EF Core + PostgreSQL + şifreleme + kimlik çözücü
builder.Services.AddInfrastructure(builder.Configuration);

// Trendyol istemcisi
builder.Services.Configure<TrendyolOptions>(builder.Configuration.GetSection(TrendyolOptions.SectionName));
builder.Services.AddHttpClient<ITrendyolApiClient, TrendyolApiClient>((sp, http) =>
{
    var baseUrl = builder.Configuration[$"{TrendyolOptions.SectionName}:BaseUrl"]
                  ?? "https://apigw.trendyol.com/integration/";
    http.BaseAddress = new Uri(baseUrl);
});

// Uygulama servisleri
builder.Services.AddScoped<ProfitCalculator>();
builder.Services.AddScoped<ReconciliationService>();
builder.Services.AddScoped<DeadStockService>();
builder.Services.AddScoped<RealPriceService>();
builder.Services.AddScoped<HealthScoreCalculator>();
builder.Services.AddScoped<CogsCsvParser>();

// Ingestion
builder.Services.AddScoped<SettlementIngestionService>();
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
app.MapControllers();
app.UseHangfireDashboard("/hangfire");

// Saatlik artımlı senkron — tüm aktif mağazalar.
RecurringJob.AddOrUpdate<IngestionJobs>(
    "settlement-sync-hourly",
    j => j.SyncAllActiveAsync(CancellationToken.None),
    Cron.Hourly);

app.Run();

/// <summary>Geçerli tenant'ı HTTP başlığından/claim'inden çözer (MVP: "X-Tenant-Id" header).</summary>
internal sealed class HttpCurrentTenant : ICurrentTenant
{
    public Guid? TenantId { get; }

    public HttpCurrentTenant(IHttpContextAccessor accessor)
    {
        var raw = accessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(raw, out var id)) TenantId = id;
    }
}
