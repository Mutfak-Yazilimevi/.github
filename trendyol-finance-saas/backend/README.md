# Backend — TrendyolFinance (.NET 8)

Çoklu kiracılı (multi-tenant) finans & kârlılık zekâsı API'si. Katmanlı (Clean
Architecture benzeri) yapı.

## Katmanlar

| Proje | Sorumluluk |
|-------|-----------|
| `Domain` | Saf alan modeli (entity, enum, kurallar). Dış bağımlılık yok. |
| `Application` | Kullanım senaryoları: `ProfitCalculator` (F1/F2), `ReconciliationService` (F3). |
| `Integration` | Trendyol API istemcisi (Basic Auth, pencere dilimleme), TÜİK sağlayıcısı. |
| `Infrastructure` | EF Core `AppDbContext`, PostgreSQL, tenant global filtresi. |
| `Api` | ASP.NET Core Web API, controller'lar, DI, Swagger. |
| `tests/UnitTests` | xUnit — kâr hesabı ve pencere dilimleme testleri. |

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

1. EF Core migration + tenant şifreleme dönüştürücüsü.
2. `Ingestion` projesi: Hangfire job'larıyla settlement backfill + artımlı çekme.
3. COGS girişi: manuel CRUD + Excel/CSV import endpoint'i + muhasebe adaptörü arayüzü.
4. Reel fiyat (TÜİK) ve ölü stok raporları.
