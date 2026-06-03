# Trendyol Finans & Kârlılık Zekâsı

Trendyol satıcıları için **finansal analitik & iş zekâsı** SaaS ürünü.
Operasyonel entegrasyon (ürün/stok/sipariş senkronu) değil; odak **kâr, hakediş
mutabakatı, reel fiyat ve öngörü**.

> "İşletmem gerçekten kâr ediyor mu, ne kadar, ve yarın ne olacak?"

## Yapı

```
trendyol-finance-saas/
├─ docs/        # mimari tasarım + API sözleşmesi
├─ backend/     # .NET 8 (API + katmanlar + testler)
├─ web/         # Angular yönetim paneli (bootstrap bekliyor)
└─ mobile/      # React Native — Android + iOS (bootstrap bekliyor)
```

## Özellikler (hedef)

| Kod | Özellik | Durum |
|-----|---------|-------|
| F1 | Ürün/kategori/dönem gerçek kâr (COGS dahil) | İskelet (ProfitCalculator + endpoint) |
| F2 | Aylık/yıllık kârlılık & sağlık skoru | Planlı |
| F3 | Hakediş mutabakatı ("Trendyol doğru mu hesaplamış?") | İskelet (ReconciliationService) |
| F4 | Enflasyona göre reel fiyat erimesi (TÜİK) | Planlı |
| F5 | Ölü stok / yavaş satan tespiti | Planlı |
| F6 | Öngörü / forecast | Planlı |
| F7 | Pazar araştırması & pazarlama önerileri | Planlı |

## Kararlar
- **Model:** Multi-tenant SaaS · **Backend:** .NET 8 + PostgreSQL · **Web:** Angular · **Mobil:** React Native
- **COGS girişi:** manuel UI + Excel/CSV import + muhasebe entegrasyonu (üçü de)

## Önemli not
Bu iskelet, `dotnet`/Angular/RN CLI **bulunmayan** bir ortamda elle oluşturuldu;
kod **derlenip doğrulanmadı**. Yerelde derleme/çalıştırma adımları her alt klasörün
`README.md`'sindedir. Detaylı mimari: [`docs/01-mimari-ve-tasarim.md`](docs/01-mimari-ve-tasarim.md).
