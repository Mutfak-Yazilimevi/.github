# API Sözleşmesi (taslak)

Web (Angular) ve mobil (React Native) istemcilerinin paylaştığı backend sözleşmesi.
Tüm istekler `X-Tenant-Id` header'ı (MVP) veya JWT (üretim) ile tenant kapsamındadır.

## Kimlik & ortak
- `Authorization: Bearer <jwt>` (üretim)
- `X-Tenant-Id: <guid>` (MVP)
- Para alanları `decimal`, para birimi `TRY` varsayılan.

## Kârlılık (F1/F2)
```
GET /api/profit/summary?from=2026-01-01&to=2026-01-31
→ 200 {
    grossRevenue, commission, deductions, cogs,
    netRevenue, netProfit, marginPercent, cogsAvailable
  }
```
> `cogsAvailable=false` ise `netProfit` gösterilmez; UI "maliyet girin" uyarısı verir.

## Hakediş Mutabakatı (F3)
```
GET /api/reconciliation?from=...&to=...
→ 200 [ { trendyolTransactionId, orderNumber,
          expectedCommission, actualCommission, deviation, severity } ]
```

## COGS (üç yöntem)
```
POST   /api/cogs                 # manuel tek kayıt
POST   /api/cogs/import          # Excel/CSV (multipart) → satır doğrulama raporu
GET    /api/cogs/{productId}     # ürünün maliyet geçmişi (effective-dated)
```

## Reel Fiyat (F4) / Ölü Stok (F5) / Öngörü (F6)
```
GET /api/pricing/real?productId=...        # nominal vs reel (TÜFE) seri
GET /api/inventory/dead-stock?days=90      # N gündür satılmayanlar + bağlı sermaye
GET /api/forecast/sales?productId=...      # ileri dönem tahmin
```

> Bu sözleşme iskelettir; alanlar implementasyonla netleşecek. Backend'de yalnızca
> `/api/profit/summary` örnek olarak uygulanmıştır.
