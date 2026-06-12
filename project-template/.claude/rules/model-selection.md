# Model Selection — İşe Bağlı Model Kararı

> **İlke:** en az token + ortalama-üstü performans. Her iş için modeli **bilinçli** seç — varsayılana
> körü körüne yapışma. Üç kademe: `haiku` · `sonnet` (varsayılan) · `opus`.

## Karar tablosu (iş → model)

| İş sınıfı | Model | Örnek |
| :--- | :--- | :--- |
| **Mekanik / yapılandırılmış / yüksek-hacim, düşük muhakeme** | `haiku` | format/lint, basit dönüşüm, kataloglama, çeviri, doküman taslağı, kurallı/sığ review, test iskeleti |
| **Ciddi muhakeme / mimari / kod yazımı / debug / uzmanlık** (VARSAYILAN) | `sonnet` | feature geliştirme, mimari tasarım, hata ayıklama, güvenlik denetimi, çoğu agent |
| **En karmaşık orkestrasyon / çok-adımlı planlama / belirsiz-geniş problem / zor mimari** | `opus` | tech-lead orkestrasyon, çok-ajanlı koordinasyon, riskli/geri-dönülmez geniş kararlar |

## Karar kontrol listesi (hızlı sınıflama)

1. **Muhakeme derinliği?** düşük → `haiku` · orta/yüksek → `sonnet` · ekstrem/orkestrasyon → `opus`
2. **Hata maliyeti / geri-dönülmezlik?** yüksek → en az `sonnet`
3. **Hacim / tekrar?** yüksek hacim + basit kural → `haiku`
4. **Bağlam genişliği / çok-adımlılık?** çok geniş/çok adımlı → `opus`

> Şüphedeyken `sonnet`. İş beklenenden karmaşık çıkarsa **yükselt**; mekanikleşince **düşür**.

## Nasıl uygulanır

- **Agent `model` alanı:** agent üret/atarken bu tabloya göre seç (`capability-gaps.md`). Katalogdaki
  `model` kolonu (`agents-catalog.csv`) bu kararı yansıtır.
- **Çalışma modeli (session):** bir göreve başlarken işi sınıfla → uygun modele geç (`/model`).
  Opus'ta hız için fast mode (`/fast`) — küçük modele düşürmez.
- **Katalog-öncelikli akışta:** skill/agent'ı bulduktan sonra (`catalog.md`) **modeli de** bu kurala
  göre belirle; agent'a devrediyorsan agent'ın kendi `model`'i geçerlidir.

## Mevcut atama notu

Dağılım: `haiku` 10 · `sonnet` 53 · `opus` 1 — ilkeyle uyumlu. `agent-organizer` (üst-düzey
orkestratör) ve `architect-reviewer` (mimari muhakeme) `haiku`→`sonnet` hizalandı. Tek `opus`:
`tech-lead-orchestrator` (en karmaşık orkestrasyon).

## Related
- Skill/agent seçimi: `catalog.md` · Agent üretimi: `capability-gaps.md` · Süreç: `process.md`.
