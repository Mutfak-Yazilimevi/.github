# Backend — TrendyolFinance (.NET 8)

Çoklu kiracılı (multi-tenant) finans & kârlılık zekâsı API'si. Katmanlı (Clean
Architecture benzeri) yapı.

## Katmanlar

| Proje | Sorumluluk |
|-------|-----------|
| `Domain` | Saf alan modeli (entity, enum, kurallar). Dış bağımlılık yok. |
| `Application` | Kullanım senaryoları: `ProfitCalculator` (F1/F2), `ReconciliationService` (F3). |
| `Integration` | Trendyol API istemcisi (Basic Auth, pencere dilimleme), TÜİK sağlayıcısı. |
| `Infrastructure` | EF Core `AppDbContext`, PostgreSQL, tenant global filtresi, kimlik şifreleme (Data Protection). |
| `Ingestion` | Hangfire job'ları: settlement backfill + saatlik artımlı senkron (idempotent upsert). |
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
| `POST /api/cogs`, `POST /api/cogs/import`, `GET /api/cogs/{id}` | COGS: manuel + Excel/CSV + geçmiş |
| `/hangfire` | Arka plan iş panosu |

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
- **Tenant çözümü** MVP'de `X-Tenant-Id` header'ı ile; üretimde JWT claim'i.
- **API sırları** (`ApiKeyEncrypted`/`ApiSecretEncrypted`) şifreli saklanır;
  şifreleme dönüştürücüsü Infrastructure'da eklenecek (Data Protection / KMS).

## Sıradaki adımlar (kod)

1. **EF Core migration**: `dotnet ef migrations add Initial -p src/TrendyolFinance.Infrastructure -s src/TrendyolFinance.Api`.
2. **TÜİK içe aktarımı**: TÜFE serisini `InflationIndex`'e dolduran job + kategori komisyon senkronu (`CategoryCommission`).
3. **Muhasebe entegrasyonu** adaptörü (Paraşüt/Logo/Mikro) → `CostSource.AccountingIntegration`.
4. **Forecast (F6)** ve **öneriler (F7)** servisleri.
5. **JWT auth + abonelik**; tenant çözümünü header yerine claim'e taşı.
6. **Hangfire.PostgreSql** depolama ayarını kurulu sürüme göre teyit et (string overload vs `UseNpgsqlConnection`).
