# `output-styles/` — Çıktı Stilleri

Claude'un **nasıl yanıt verdiğini** (ton, format, ayrıntı düzeyi) tanımlayan markdown dosyaları.
Her dosya bir stil; `/output-style <ad>` ile seçilir. Stil *ne yaptığını* değil, *nasıl ilettiğini*
değiştirir (kurallar `rules/`'da, davranış burada).

## Format

```markdown
---
name: concise-tr
description: Kısa, Türkçe, eyleme dönük yanıtlar
---
<sistem talimatına eklenecek stil yönergeleri>
```

## Örnek

- [`concise-tr.md`](concise-tr.md) — kısa, Türkçe, madde-odaklı.

> Stil **opsiyoneldir**; seçilmezse Claude varsayılan davranışını kullanır. Proje-özel bir
> iletişim standardı dayatmak istemiyorsan bu klasörü boş bırakabilirsin.
