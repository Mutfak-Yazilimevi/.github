# Backend — TrendyolFinance (.NET 8)

Çoklu kiracılı (multi-tenant) finans & kârlılık zekâsı API'si. Katmanlı (Clean
Architecture benzeri) yapı.

## Katmanlar

| Proje | Sorumluluk |
|-------|-----------|
| `Domain` | Saf alan modeli (entity, enum, kurallar). Dış bağımlılık yok. |
| `Application` | Use-case'ler + port'lar (ProfitCalculator, Reconciliation, Analytics, Cogs, `IInflationProvider`, `IAccountingProvider`). Dış bağımlılık yok. |
| `Integration` | Adaptörler: Trendyol finans/katalog istemcisi, TÜİK (EVDS) TÜFE, Paraşüt muhasebe. (Bağımlılık yönü: Integration → Application) |
| `Infrastructure` | EF Core `AppDbContext`, PostgreSQL, tenant global filtresi, kimlik şifreleme (Data Protection), design-time factory. |
| `Ingestion` | Hangfire job'ları: settlement backfill + saatlik senkron, TÜFE (aylık), kategori komisyon (günlük), muhasebe maliyet içe aktarımı. |
| `Api` | ASP.NET Core Web API, controller'lar, DI, Swagger, Hangfire dashboard. |
| `tests/UnitTests` | xUnit — kâr, mutabakat, ölü stok, reel fiyat, CSV import, pencere dilimleme. |

## Mevcut endpoint'ler

| Endpoint | Özellik |
|----------|---------|
| `POST /api/selleraccounts/connect` | Mağaza bağla (kimlik şifreli), backfill'i tetikle |
| `GET  /api/profit/summary` | Dönemsel kârlılık (F1/F2) |
| `GET  /api/reconciliation` | Hakediş komisyon sapmaları (F3) |
| `GET  /api/pricing/real` | Enflasyona göre reel fiyat (F4) |
| `GET  /api/inventory/dead-stock` | Ölü stok / yavaş satan (F5) |
| `GET  /api/forecast/sales` | Satış öngörüsü (F6) |
| `GET  /api/recommendations` | Pazarlama/fiyat önerileri (F7) |
| `GET  /api/dashboard` | Sağlık skoru + KPI özeti (F2) |
| `POST /api/cogs`, `POST /api/cogs/import`, `GET /api/cogs/{id}` | COGS: manuel + Excel/CSV + geçmiş |
| `/hangfire` | Arka plan iş panosu (saatlik settlement, günlük komisyon + ürün/fiyat, aylık TÜFE) |

## Yapılandırma (appsettings)

| Bölüm | Anahtar | Açıklama |
|-------|---------|----------|
| `Jwt` | `Authority`, `Audience` | Kimlik sağlayıcısı (boşsa dev'de `X-Tenant-Id` header'ı kullanılır) |
| `Trendyol` | `BaseUrl` | API taban adresi (resmî dokümandan teyit) |
| `Tuik` | `ApiKey`, `SeriesCode` | TCMB EVDS anahtarı + TÜFE seri kodu |
| `Parasut` | `CompanyId`, `AccessToken` | Muhasebe entegrasyonu (iskelet) |

## Migration

```bash
# AppDbContextFactory sayesinde tasarım zamanı çalışır:
dotnet ef migrations add Initial -p src/TrendyolFinance.Infrastructure -s src/TrendyolFinance.Api
dotnet ef database update -p src/TrendyolFinance.Infrastructure -s src/TrendyolFinance.Api
```

## Çalıştırma (yerel, .NET 8 SDK gerekir)

```bash
cd backend
./generate-solution.sh          # .sln üretir (bu ortamda dotnet yoktu, o yüzden script ile)
dotnet build
dotnet test
# PostgreSQL ayağa kalkınca:
dotnet run --project src/TrendyolFinance.Api
# Swagger: https://localhost:5001/swagger
```

## Önemli notlar

- **dotnet SDK bu geliştirme ortamında yoktu**, dolayısıyla kod derlenip
  doğrulanamadı. Paket sürümleri ve uç nokta yolları (`Integration`) yerelde
  derlenirken teyit edilmeli.
- **Trendyol uç noktaları** (`TrendyolApiClient`) resmî dokümandan birebir
  doğrulanacak — portal bu ortamdan 403 verdiği için canlı teyit yapılamadı.
  İşaretli yerler: `BaseUrl`, settlement yolu, alan adları.
- **Tenant çözümü** JWT `tenant_id` claim'inden; dev'de `X-Tenant-Id` header'ı fallback.
- **API sırları** (`ApiKeyEncrypted`/`ApiSecretEncrypted`) Data Protection ile şifreli; üretimde anahtar halkası kalıcı depo/KMS'e alınmalı.

## Sıradaki adımlar (kod)

1. **EF migration üret + DB'yi kur** (yukarıdaki komutlar).
2. **TÜİK EVDS** yanıt şemasını/seri kodunu canlı doğrula; **Paraşüt** OAuth2 + alış faturası çekimini tamamla.
3. **Ürün/fiyat senkronu**: Trendyol ürün servisinden `Product` + günlük `PriceSnapshot` doldur (F4/F5 verisi).
4. **Abonelik & faturalandırma** (plan limitleri), controller'lara `[Authorize]` + rol politikaları.
5. **Hangfire.PostgreSql** depolama çağrısını kurulu sürüme göre teyit et (string overload vs `UseNpgsqlConnection`).
