# 0001 — Clean Architecture + Vertical Slice + CQRS

- **Statü:** Accepted
- **Tarih:** 2026-06-12
- **Karar verenler:** Mutfak Yazılımevi

## Bağlam

.NET 10 REST API; uzun ömürlü, iş kuralı yoğun, test edilebilir kalması gereken bir backend.
Hem katman disiplini (bağımlılık yönü), hem feature bazlı geliştirme hızı, hem de okuma/yazma
yüklerinin ayrı evrilebilmesi isteniyor. Tek bir stil bu üç ihtiyacı tek başına karşılamıyor.

## Karar

Üç deseni birlikte kullanıyoruz:
- **Clean Architecture** — bağımlılık içe doğru: `WebApi → Infrastructure → Application → Domain`.
- **Vertical Slice** — her feature uçtan uca kendi slice'ında (Command/Query + Handler + Validator + Endpoint).
- **CQRS** — yazma (command) ve okuma (query) modelleri ayrı.

## Sonuçlar

**Artı:**
- Domain saf ve test edilebilir; framework/DB/UI uca itilir.
- Feature'lar yüksek kohezyon, düşük cross-feature kuplaj → paralel geliştirme.
- Okuma/yazma bağımsız optimize/ölçeklenebilir.

**Eksi / ödünler:**
- Daha fazla başlangıç kurulumu ve öğrenme eğrisi.
- Slice tutarlılığı için ekip disiplini gerekir.
- CQRS, basit CRUD feature'lar için fazla gelebilir (gerektiğinde sade tut).

## Değerlendirilen alternatifler

- **Düz N-Tier** — katman ayrımı verir ama kuplaj yüksek, feature bazlı hız düşük.
- **Saf monolit/katmanlı** — MVP için hızlı ama uzun vadeli test edilebilirlik zayıf.
- **Microservices** — bu ölçek/ekip için erken; operasyonel karmaşıklık fazla.

> İlgili: `.claude/rules/architecture.md`, `dev-dotnet-architecture-selection` skill'i.
