# Trendyol Finans & Kârlılık Zekâsı — Mimari ve Tasarım Dokümanı

> Trendyol satıcıları için **finansal analitik & iş zekâsı** SaaS ürünü.
> Operasyonel entegrasyon (ürün/stok/sipariş senkronu) DEĞİL; odak **para, kâr,
> mutabakat, reel fiyat ve öngörü**.

**Durum:** Taslak (v0.1) · **Tarih:** 2026-06-03 · **Stack:** .NET (C#) + PostgreSQL · **Model:** Multi-tenant SaaS

---

## 1. Ürün Vizyonu

Trendyol satıcısının tek bir soruya net cevap almasını sağlamak:
**"İşletmem gerçekten kâr ediyor mu, ne kadar, ve yarın ne olacak?"**

Mevcut entegratörler operasyona odaklı. Bu ürün **finansal gerçeği** ortaya
koyar: gerçek kâr (maliyet dahil), Trendyol hakedişinin doğruluğu, enflasyona
karşı reel fiyat, ölü stok ve geleceğe dönük öngörü.

### Hedef özellikler (kullanıcının talebi)

| # | Özellik | Değer önermesi |
|---|---------|----------------|
| F1 | Ürün/kategori/dönem bazında **gerçek kâr** | "Bu üründen ne kadar kâr ettim?" |
| F2 | Aylık/yıllık **kârlılık & trend** | "İşletmem iyi mi, kötü mü?" |
| F3 | **Hakediş mutabakatı** | "Trendyol doğru mu hesaplamış?" |
| F4 | **Enflasyona göre reel fiyat erimesi** | "Ürünün fiyatı eridi mi?" |
| F5 | **Ölü stok / yavaş satan** tespiti | "Hangi ürünler para bağlıyor?" |
| F6 | **Öngörü & forecast** (geçmiş→gelecek) | "Önümüzdeki ay ne beklemeliyim?" |
| F7 | **Pazar araştırması & pazarlama önerileri** | "Ne yapmalıyım?" |

---

## 2. Mimari Genel Bakış

Olay tabanlı, katmanlı, multi-tenant bir SaaS. Trendyol API'sinden veri
**periyodik olarak çekilip kendi veri ambarımızda saklanır** (canlı sorgu
değil — sebebi §4'te).

```
┌──────────────────────────────────────────────────────────────────┐
│                     Frontend (Angular / React)                     │
│         Dashboard · Kârlılık · Mutabakat · Öngörü · Öneriler        │
└───────────────────────────────┬────────────────────────────────────┘
                                 │ REST / JWT
┌───────────────────────────────▼────────────────────────────────────┐
│                    API Gateway (ASP.NET Core)                       │
│              AuthN/Z · Tenant resolution · Rate limit               │
└───────┬───────────────┬───────────────┬───────────────┬────────────┘
        │               │               │               │
 ┌──────▼─────┐  ┌──────▼──────┐  ┌─────▼──────┐  ┌──────▼──────┐
 │ Finance &  │  │ Reconcile   │  │ Pricing &  │  │ Forecast &  │
 │ Profit Svc │  │ (Hakediş)   │  │ Inflation  │  │ Insights    │
 └──────┬─────┘  └──────┬──────┘  └─────┬──────┘  └──────┬──────┘
        └───────────────┴───────┬───────┴────────────────┘
                                │
                  ┌─────────────▼──────────────┐
                  │   Data Warehouse (Postgres) │
                  │  settlements · transactions │
                  │  products · cogs · prices   │
                  └─────────────▲──────────────┘
                                │ yazar
              ┌─────────────────┴──────────────────┐
              │   Ingestion Worker (BackgroundSvc)   │
              │  Trendyol API → normalize → store    │
              │  Hangfire/Quartz zamanlanmış işler   │
              └─────────────────▲──────────────────┘
                                │ HTTPS (Basic Auth)
              ┌─────────────────┴──────────────────┐
              │  Trendyol Marketplace API + TÜİK     │
              └──────────────────────────────────────┘
```

### Önerilen teknoloji yığını (org standardıyla uyumlu)

| Katman | Seçim | Gerekçe |
|--------|-------|---------|
| Backend | **.NET 8 (ASP.NET Core Web API)** | Org standardı; uzun destek (LTS) |
| Veritabanı | **PostgreSQL** | Org standardı; zaman serisi & analitik için güçlü (window fn, partitioning) |
| ORM | **EF Core** + Dapper (ağır raporlar için) | CRUD'da EF, ağır agregasyonda ham SQL |
| Arka plan işleri | **Hangfire** | Zamanlanmış ingestion + dashboard ile izleme |
| Önbellek | **Redis** | Tenant bazlı rapor cache, rate-limit sayaçları |
| Frontend | **Angular** veya **React** | Org standardı |
| Dağıtım | **Docker** + CI/CD | Org standardı |
| Auth | JWT + ASP.NET Identity | Multi-tenant kullanıcı/abonelik |

### Solution yapısı (öneri)

```
TrendyolFinance.sln
├─ src/
│  ├─ TrendyolFinance.Api            # ASP.NET Core Web API (controllers)
│  ├─ TrendyolFinance.Application    # Use-case'ler, CQRS handler'ları, DTO
│  ├─ TrendyolFinance.Domain         # Entity'ler, value object, domain kuralları
│  ├─ TrendyolFinance.Infrastructure # EF Core, repository, dış servis client
│  ├─ TrendyolFinance.Integration    # Trendyol API client + TÜİK client
│  ├─ TrendyolFinance.Ingestion      # Hangfire job'ları, ETL pipeline
│  └─ TrendyolFinance.Worker         # Background host
└─ tests/
   ├─ TrendyolFinance.UnitTests
   └─ TrendyolFinance.IntegrationTests
```

---

## 3. Trendyol API — Finans Endpoint Eşleştirmesi

> Doğrulanması gereken (portal 403 verdiği için canlı teyit edilemedi — entegrasyon
> bilgileri panelinden ve resmî dokümandan kesinleştirilecek).

### Kimlik doğrulama
- **Basic Auth**: `Authorization: Basic base64(ApiKey:ApiSecret)`
- Header: `User-Agent: {SellerId} - SelfIntegration` (Trendyol bunu zorunlu tutar)
- Her tenant kendi `SellerId`, `ApiKey`, `ApiSecret` değerlerini girer (şifreli saklanır).

### Kullanılacak servisler (finans odaklı)

| Servis | Amaç | Beslediği özellik |
|--------|------|-------------------|
| **Settlements** (Hakediş kalemleri) | Satış, İade, İndirim, Kupon, Kargo, Komisyon kalemleri | F1, F2, F3 |
| **Other Financials** | Komisyon dışı kesintiler, ek ücretler, ceza/iadeler | F2, F3 |
| **Orders / Shipment Packages** | Sipariş & paket bazlı tutar, komisyon, kargo | F1, F3 |
| **Products** | SKU, barkod, kategori, güncel fiyat & stok | F1, F4, F5 |
| **Categories & Commission** | Kategori komisyon oranları | F3 (beklenen komisyon hesabı) |
| **Claims / Returns** | İade detayları | F1, F2 |

> **Settlement kalem tipleri** (mutabakatın çekirdeği): `Sale`, `Return`,
> `Discount`, `DiscountCancel`, `Coupon`, `CouponCancel`, `ProvisionPositive/Negative`,
> `ManualRefund`, `Stoppage` vb. Her kalemde: satış tutarı, komisyon tutarı,
> komisyon oranı, satıcı net kazancı.

---

## 4. Veri Çekme (Ingestion) Stratejisi — neden kendi ambarımız?

Trendyol finans API'sinde:
- **Tarih aralığı limiti** var (settlement sorguları tipik olarak ≤ 15 gün).
- **Rate limit** var (servis bazlı).
- Geçmiş veri tek seferde değil, sayfalı (paginated) gelir.

Bu nedenle **canlı sorgu yapamayız**; düzenli çalışan bir ingestion katmanı veriyi
normalize edip ambara yazar. Analitik, mutabakat ve öngörünün tamamı bu birikmiş
veri üzerinden çalışır.

**İş akışı (Hangfire):**
1. **İlk yükleme (backfill):** Tenant bağlanınca son 12–24 ayı 15 günlük pencerelerle çek.
2. **Artımlı (incremental):** Her saat/gün son pencereyi çek, idempotent upsert.
3. **Fiyat fotoğrafı (snapshot):** Ürün fiyatlarını günlük kaydet → F4 için fiyat geçmişi.
4. **Hata/yeniden deneme:** Üstel backoff, kalem bazlı dedup (Trendyol transaction id ile).

---

## 5. Veri Modeli (çekirdek)

```
Tenant ─┬─< SellerAccount (Trendyol kimlik bilgileri, şifreli)
        └─< AppUser (roller, abonelik)

SellerAccount ─┬─< SettlementTransaction   # ham hakediş kalemleri (idempotent)
               ├─< Order ─< OrderLine
               ├─< Product ─┬─< PriceSnapshot      # F4: fiyat geçmişi
               │            └─< ProductCost (COGS)  # KULLANICI GİRER (F1)
               └─< IngestionRun            # ETL audit log
```

### Kritik tablolar

**SettlementTransaction** (mutabakat & kârın temeli)
```
Id, SellerAccountId, TrendyolTransactionId (unique), TransactionType,
OrderNumber, Barcode, TransactionDate,
GrossAmount, CommissionRate, CommissionAmount, SellerRevenue,
Currency, RawJson, IngestedAt
```

**ProductCost (COGS)** — *Trendyol vermez, kullanıcı girer/import eder*
```
Id, ProductId, EffectiveFrom, EffectiveTo,
PurchasePrice, VatRate, PackagingCost, OtherCost, Source(Manual/Import)
```
> Tarih aralıklı (effective-dated): maliyet zamanla değişir; geçmiş kâr doğru
> hesaplanmalı (satış anındaki maliyet kullanılır).

**PriceSnapshot** (reel fiyat erimesi)
```
Id, ProductId, CapturedAt, ListPrice, SalePrice, Currency
```

**InflationIndex** (TÜİK TÜFE — tenant'tan bağımsız, paylaşımlı)
```
YearMonth, CpiValue, Source
```

---

## 6. Özellik Modülleri — hesap mantığı

### F1 · Gerçek Kâr
```
NetKar(ürün, dönem) =
    Σ SellerRevenue (satış)
  − Σ CommissionAmount
  − Σ İade/iptal etkisi
  − Σ Kargo & hizmet kesintileri
  − Σ (satış adedi × satış anındaki COGS)
  − (opsiyonel) reklam gideri
```
> **Bağımlılık:** COGS girilmemişse "kâr" değil yalnızca "net ciro" gösterilir;
> UI bunu net şekilde belirtir.

### F2 · Kârlılık & Trend
- Aylık/yıllık ciro, komisyon, net kâr, kâr marjı (window fonksiyonları).
- "İşletme sağlık skoru": marj trendi + iade oranı + stok devir hızı + büyüme birleşik skoru.

### F3 · Hakediş Mutabakatı  ⭐ farklılaştırıcı
```
BeklenenKomisyon = GrossAmount × KategoriKomisyonOranı
Sapma = |Trendyol.CommissionAmount − BeklenenKomisyon|
Sapma > eşik  → işaretle + kalem detayını göster
```
- Eksik/yinelenen kalem tespiti (sipariş var, hakediş yok vb.).
- **Uyarı:** Kampanya komisyonu & özel anlaşmalar oranı değiştirir. MVP'de "büyük
  sapmaları yakala", zamanla kural setini zenginleştir (yanlış pozitifleri azalt).

### F4 · Enflasyona Göre Reel Fiyat
```
ReelFiyat(t) = NominalFiyat(t) × (CPI_baz / CPI_t)
Erime % = (ReelFiyat_şimdi − Fiyat_baz) / Fiyat_baz
```
- PriceSnapshot + TÜİK TÜFE serisi. "Bu ürünü 6 ay önceki alım gücüne göre %X ucuza satıyorsun."

### F5 · Ölü Stok / Yavaş Satan
- SKU bazında satış hızı (son N gün adet), gün sayısı (days-since-last-sale).
- Bağlı sermaye = stok adedi × COGS. Sıralı "para bağlayan ürünler" listesi.

### F6 · Öngörü
- Zaman serisi: satış adedi & kâr. Sezonsallık + trend.
- MVP: hareketli ortalama / üstel düzleştirme; sonra ML.NET veya harici tahmin servisi.

### F7 · Pazar Araştırması & Öneriler
- **Resmî API'de rakip fiyatı yok** → MVP'de kendi veriyle: düşük marjlı/erimiş
  fiyatlı ürünler için fiyat artışı, ölü stok için kampanya/indirim önerisi.
- Rakip verisi (gri alan/scraping) ileride ayrı, opsiyonel, riskleri değerlendirilmiş bir modül.

---

## 7. Multi-Tenancy & Güvenlik

- **İzolasyon:** Tüm sorgularda `TenantId` zorunlu (global query filter); ileride
  ölçek için şema/DB ayrımına geçilebilir.
- **Sırlar:** Trendyol ApiKey/Secret **şifreli** saklanır (Data Protection / KMS).
- **Yetki:** Rol bazlı (Sahip / Muhasebe / Görüntüleyici).
- **Denetim:** Ingestion ve mutabakat sonuçları audit log'lanır.
- **KVKK:** Finansal veri; saklama & silme politikaları tanımlanır.

---

## 8. Yol Haritası (aşamalı)

| Faz | İçerik | Çıktı |
|-----|--------|-------|
| **0** | Solution iskeleti, multi-tenant temel, Trendyol client + auth | Çalışan iskelet |
| **1** | Ingestion (settlement/order/product) + Sağlık Panosu (F2) | İlk "vay be" |
| **2** | COGS girişi + Gerçek Kâr (F1) + Ölü Stok (F5) | Kâr netliği |
| **3** | Hakediş Mutabakatı (F3) + Reel Fiyat (F4, TÜİK) | Farklılaştırıcı |
| **4** | Öngörü (F6) + Öneriler (F7) | Akıllı katman |

> Kullanıcı talebi "hepsi" → hedef tüm fazlar; ama tek temel üstüne **sırayla**
> teslim ederek her aşamada kullanılabilir değer üretiriz.

---

## 9. Kararlar ve Açık Konular

### Verilen kararlar (2026-06-03)
- **COGS girişi:** Üç yöntem de desteklenecek → (a) manuel UI, (b) Excel/CSV import, (c) muhasebe entegrasyonu (ileride). Veri modeli `ProductCost.Source` alanıyla kaynağı ayırır.
- **Web Frontend:** **Angular**.
- **Mobil:** **React Native** (Android + iOS) — tek kod tabanı.
- **Repo:** Şimdilik iskelet bu repoda `trendyol-finance-saas/` altında; ürünleşince adanmış repoya taşınır.

### Açık konular
1. **Reklam gideri** kâra dahil edilecek mi? (Trendyol reklam API'si ayrı yetki ister)
2. **Rakip/pazar verisi** kapsamı ve hukuki sınırlar (scraping riski).
3. Trendyol finans endpoint'lerinin **birebir teyidi** (alan adları, kalem tipleri, limitler).
4. Muhasebe entegrasyonu: **ilk adaptör Paraşüt** seçildi (`IAccountingProvider` ile genişletilebilir; Logo/Mikro sonra). OAuth2 + alış faturası çekimi tamamlanacak.

---

## 10. Çözüm Yapısı (bu repodaki iskelet)

```
trendyol-finance-saas/
├─ docs/                      # bu doküman
├─ backend/                   # .NET 8 çözümü (API + Worker + katmanlar)
├─ web/                       # Angular yönetim paneli
└─ mobile/                    # React Native (Android + iOS)
```

> Bu ortamda `dotnet`/Angular CLI bulunmadığından dosyalar elle oluşturuldu;
> her klasördeki `README.md` derleme/çalıştırma adımlarını ve (gerekiyorsa)
> CLI ile yeniden üretme komutlarını içerir.
