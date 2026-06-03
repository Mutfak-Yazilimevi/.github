using Microsoft.EntityFrameworkCore;
using TrendyolFinance.Application.Finance;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Integration.Trendyol;

var builder = WebApplication.CreateBuilder(args);

// --- Servisler ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Çoklu kiracı: geçerli tenant (MVP'de header'dan; üretimde JWT claim'inden).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, HttpCurrentTenant>();

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
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
