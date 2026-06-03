<p align="center"><b>🐂 Leather ERP — Deri Üretim Yönetimi</b></p>
<p align="center"><i>Deri stok takibi · maliyet & fire hesabı · ürün fiyatlandırma · mamul takibi</i></p>

---

Deri ürün üreticileri için geliştirilen; **deri stoğu**, **üretim maliyeti**, **fire (kesim kaybı)**,
**fiyatlandırma** ve **eldeki mamul** süreçlerini tek yerden yöneten bir uygulama.

**Teknoloji:** .NET 8 Web API · Angular 18 (Material) · PostgreSQL · Docker

## Modüller (MVP)

| Modül | Ne yapar |
| :-- | :-- |
| **Deri Stok** | Deri tipi tanımı, lot bazlı giriş (dm²/ayak²), kalan miktar, stok değeri, düşük stok uyarısı |
| **Maliyet & Fire** | Ürün reçetesi (net deri + fire + işçilik + genel gider) → şeffaf birim maliyet kırılımı |
| **Fiyatlandırma** | Maliyet üzerine kâr marjı + KDV → satış fiyatı; hedef fiyattan kâr marjı çözme |
| **Üretim & Mamul** | Üretim emri → deri stoğundan FIFO düşüm → eldeki mamul stoğu (ağırlıklı ort. maliyetle) |

## Hesaplama konvansiyonları

- **Birim:** Deri miktarları sistemde **kanonik dm²** saklanır; arayüzde dm² veya ayak² (1 ayak² = 9.2903 dm²) gösterilir.
- **Fire:** Net deri üzerine eklenir → `brüt = net × (1 + fire)`. Örn. %15 fire, 8 dm² net → 9.2 dm² tüketim.
- **Maliyet:** `birim maliyet = brüt deri × deri birim mlt. + işçilik + genel gider`.
- **Fiyat:** `satış = birim maliyet × (1 + kâr marjı) × (1 + KDV)`.
- **Üretim:** Deri en eski lottan başlanarak (**FIFO**) düşülür; gerçekleşen maliyet tüketilen lotların fiyatına göre hesaplanır.

## Hızlı başlangıç (Docker)

```bash
cp .env.example .env        # değerleri düzenleyin
docker compose up --build
```

- Web arayüzü: http://localhost:8080
- API + Swagger: http://localhost:5080/swagger
- Varsayılan kullanıcı: **admin / admin123** (ilk açılışta seed edilir)

## Yerel geliştirme

**Backend** (PostgreSQL `localhost:5432` gerektirir; bağlantı dizesi `backend/src/LeatherErp.Api/appsettings.json`):

```bash
cd backend
dotnet run --project src/LeatherErp.Api      # http://localhost:5080/swagger
dotnet test                                  # birim testleri
```

Uygulama açılışta migration + seed uygular.

**Frontend** (dev sunucusu `/api`'yi `localhost:5080`'e proxy'ler — `proxy.conf.json`):

```bash
cd frontend/leather-erp-web
npm install
npm start                                    # http://localhost:4200
```

## Proje yapısı

```
backend/   .NET 8 — Domain / Application / Infrastructure / Api katmanları + xUnit testleri
frontend/  Angular 18 — features/{stock,products,pricing,production,settings,login} + core servisleri
docker-compose.yml   postgres + api + web
```

### Mimari notlar

- **Domain:** Saf entity'ler, enum'lar ve `UnitConverter` (birim dönüşümü tek otorite).
- **Application:** Test edilebilir servisler — `CostCalculationService`, `PricingService`,
  `ProductionService` (FIFO), `StockService`; `IAppDbContext` soyutlaması.
- **Infrastructure:** EF Core + Npgsql, yapılandırmalar, migration, `DbSeeder`.
- **Api:** Controller'lar, JWT auth, Swagger, global hata middleware'i, CORS.

## Sonraki fazlar

- Native mobil uygulama (React Native, aynı API).
- Raporlama/dashboard (kârlılık, tedarikçi maliyet analizi), PDF teklif/fiş.
- Çoklu depo, parti/seri takibi, barkod; gelişmiş roller ve yetkilendirme.

## API uçları (özet)

`POST /api/auth/login` · `GET/POST /api/leather-types` · `GET/POST /api/stock/lots` ·
`GET /api/stock/levels` · `GET /api/stock/low` · `GET/POST /api/products` ·
`PUT /api/products/{id}/recipe` · `POST /api/production` · `POST /api/production/{id}/confirm` ·
`GET /api/finished-goods` · `POST /api/pricing/cost` · `POST /api/pricing/price` ·
`GET /api/pricing/product/{id}` · `GET/PUT /api/settings`

Tüm uçların ayrıntısı için Swagger UI: `/swagger`.
