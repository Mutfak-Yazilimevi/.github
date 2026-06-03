# Mobil — React Native (Android + iOS)

Satıcının cebinden işletme sağlığını izlemesi: özet KPI'lar, kâr trendi,
hakediş uyarıları (push bildirim), ölü stok hatırlatmaları.

## Bootstrap (React Native CLI gerekir)

Bu ortamda RN CLI bulunmadığından proje henüz üretilmedi. Yerelde:

```bash
cd mobile
npx @react-native-community/cli init TrendyolFinanceMobile
# veya Expo tercih edilirse:
# npx create-expo-app trendyol-finance-mobile
```

## Planlanan yapı

```
src/
├─ api/                 # backend istemcisi (X-Tenant-Id, JWT) — web ile aynı sözleşme
├─ screens/
│  ├─ DashboardScreen   # özet KPI + sağlık skoru
│  ├─ ProfitScreen      # dönemsel kâr
│  ├─ ReconciliationScreen  # hakediş sapma uyarıları
│  └─ DeadStockScreen   # ölü stok
├─ components/          # KpiCard, TrendChart, AlertBadge
├─ navigation/          # React Navigation (stack + bottom tabs)
└─ services/            # push bildirim (FCM/APNs), güvenli token saklama
```

## Öneriler
- Navigasyon: React Navigation.
- Grafik: react-native-gifted-charts veya victory-native.
- Push: FCM (Android) + APNs (iOS) — kritik hakediş sapması/ölü stok uyarıları.
- Backend sözleşmesi: `../docs/api-contract.md`.
