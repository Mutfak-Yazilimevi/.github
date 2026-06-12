# Architecture Decision Records (ADR)

Mimari açıdan önemli kararları kayıt altına aldığımız yer. Her karar bir dosya:
`NNNN-kisa-baslik.md` (sıralı numara). Kararlar **değişmez** — yerine yeni karar gelirse
eski "Superseded" işaretlenir, silinmez.

## Ne zaman ADR yazılır?

- Yapı/teknoloji/sınır değiştiren kararlar (mimari stil, DB, mesajlaşma, auth, dış servis).
- Geri alması pahalı veya ekibi uzun süre etkileyecek tercihler.
- `.claude/rules/architecture.md`'de "ADR olarak kaydet" denen durumlar.

## Nasıl?

1. `0000-template.md`'yi kopyala, sıradaki numarayı ver.
2. Bağlam → Karar → Sonuçlar (artı/eksi) → Alternatifler bölümlerini doldur.
3. Statü: `Proposed` → `Accepted` (veya `Rejected` / `Superseded by NNNN`).

## Kayıtlar

| # | Karar | Statü |
| :-- | :--- | :--- |
| [0001](0001-clean-vertical-slice-cqrs.md) | Clean Architecture + Vertical Slice + CQRS | Accepted |
