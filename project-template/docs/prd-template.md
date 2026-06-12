# PRD — <Özellik / Ürün Adı>

> Intake/backlog çıktısının bağlandığı hedef. `pm-create-prd`/`dev-to-prd` doldurur.
> Onaylanmadan tasarım/uygulama başlamaz (gate). Statü: Draft | Approved.

- **Statü:** Draft
- **Tarih:** YYYY-MM-DD
- **Kaynak:** <intake brief / backlog #B-XXX>

## 1. Amaç & kapsam
<Ne, kim için, hangi problemi çözüyor. Kapsam dışı maddeler dahil.>

## 2. Kullanıcı hikayeleri + kabul kriterleri
- **US-1:** <rol> olarak <istek> istiyorum, böylece <fayda>.
  - **Kabul:** Given <durum> → When <eylem> → Then <beklenen>.
  - **Kabul:** …
- **US-2:** …

## 3. Fonksiyonel olmayan gereksinimler (NFR)
- Performans/ölçek: <bkz. dev-system-design-scaling> · Güvenlik · Erişilebilirlik · Uyumluluk

## 4. Başarı metrikleri
<"Başarılı" nasıl ölçülür — KPI/kabul eşiği.>

## 5. Açık sorular / varsayımlar
<Netleşmemiş noktalar — uydurma, listele.>

> Onay sonrası: `spec-architect` (tasarım) → `rules/process.md` yaşam döngüsü.
