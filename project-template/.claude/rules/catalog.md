# Catalog — Katalog-Öncelikli Çalışma

> Skill ve agent'ları bulmanın **tek hızlı indeksi** kataloglardır. Her soru/plan/iş için
> **önce katalogu tara**, uygun skill/agent'ı bul, ona git. Yoksa yeni üret (`capability-gaps.md`).

## Katalog dosyaları

| Dosya | Kolonlar |
| :--- | :--- |
| `skills/skills-catalog.csv` | `name, category, subcategory, tech, path, description` |
| `agents/agents-catalog.csv` | `name, category, model, tech, path, description` |

Üretici: `claude-config/scripts/build-catalog.py` (frontmatter tarar). İnsan-okur özet:
`skills/index_skills.md`, `agents/index_agents.md`.

## Çalışma döngüsü (soru / plan / iş geldiğinde)

1. **Tara** — katalogda anahtar kelime / `tech` / `category` ile ara.
   ```bash
   grep -i "<konu>" .claude/skills/skills-catalog.csv   # amaç/açıklama/ad
   awk -F, '$4 ~ /<tek>/' .claude/skills/skills-catalog.csv   # teknolojiye göre
   awk -F, '$2=="<plugin>"' .claude/agents/agents-catalog.csv  # kategoriye göre
   ```
2. **Bul** — en uygun skill/agent('lar)ı seç (description ile teyit et).
3. **Git** — skill'i tetikle / agent'a devret; gerekirse `path` ile dosyayı aç. İşe uygun
   **modeli** de belirle (`model-selection.md`).
4. **Yoksa** — `capability-gaps.md` akışı: yeni skill/agent **öner**, onayla, üret.

> Cevaplar, planlar ve işler bu döngüyle yürütülür: önce burayı tara, bul, oraya git.

## Bakım (yeni skill/agent ekleyince — ZORUNLU)

1. **ÖNCE tara** — katalogda gerçekten yok mu? Tekrarı (duplicate) önle; varsa onu kullan.
2. **Ekle** — doğru önek + frontmatter (`capability-gaps.md` kuralları).
3. **Katalogu yeniden üret** — kaynak kütüphane güncellendikten sonra:
   ```bash
   python3 scripts/build-catalog.py     --source <.claude>   # CSV katalogları
   python3 scripts/build-skill-index.py --source <.claude>   # index_skills.md
   # agent eklediysen agents/index_agents.md'yi de elle güncelle
   ```
4. **Yayımla** — `bash scripts/publish.sh` (tüm projelere ulaşsın).

> Katalog daima kaynakla **senkron** olmalı; yeni eklenen skill/agent katalogda yoksa
> "tara→bul→git" döngüsü onu göremez.
