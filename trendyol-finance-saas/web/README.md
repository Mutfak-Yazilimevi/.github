# Web — Angular Yönetim Paneli

Satıcının finansal kontrol paneli: kârlılık, hakediş mutabakatı, reel fiyat,
ölü stok ve öngörü ekranları.

## Bootstrap (Angular CLI gerekir)

Bu ortamda Angular CLI bulunmadığından proje henüz üretilmedi. Yerelde:

```bash
npm install -g @angular/cli
cd web
ng new trendyol-finance-web --routing --style=scss --skip-git
```

## Planlanan modül yapısı

```
src/app/
├─ core/                 # http interceptor (X-Tenant-Id, JWT), auth guard
├─ shared/              # ortak bileşen, pipe (TRY para, yüzde)
├─ features/
│  ├─ dashboard/        # F2 — işletme sağlık panosu (KPI, trend grafikleri)
│  ├─ profitability/    # F1 — ürün/kategori/dönem kâr tabloları
│  ├─ reconciliation/   # F3 — hakediş sapma listesi
│  ├─ pricing/          # F4 — enflasyona göre reel fiyat
│  ├─ dead-stock/       # F5 — ölü stok / yavaş satan
│  ├─ forecast/         # F6 — öngörü grafikleri
│  └─ cogs/             # COGS girişi: manuel form + Excel/CSV import
└─ api/                 # backend ile tip güvenli istemci (bkz. docs/api-contract.md)
```

## Öneriler
- Grafik: ngx-charts veya ECharts.
- Durum yönetimi: NgRx (büyürse) ya da basit servis + signals.
- Backend sözleşmesi: `../docs/api-contract.md`.
