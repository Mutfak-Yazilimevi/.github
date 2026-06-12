# Capability Gaps — Yeni Skill/Agent Üretme

> Projede **karşılığı olmayan** (mevcut skill/agent kapsamadığı) bir konu/teknoloji/alan
> çıktığında izlenecek kural. Amaç: tek seferlik ad-hoc çözüm yerine **merkezî kütüphaneyi
> büyütmek** → bir kez yaz, tüm projeler faydalansın (tek kaynak, minimum token).

## Tetikleyici (ne zaman tavsiye ver?)

Aşağıdakilerden biri olduğunda **kullanıcıya yeni skill/agent üretmeyi öner**:
- Görev, mevcut hiçbir skill/agent'ın net kapsamadığı bir teknoloji/desen/alan içeriyor.
- Aynı boşluğu ikinci kez (veya farklı projede tekrar) çözüyorsan.
- Çözüm tekrar kullanılabilir bir prosedür, kontrol listesi veya uzmanlık personası gerektiriyor.

> Önce **öner**, kullanıcı onayıyla üret. Kullanıcı istemezse tek seferlik çöz ama boşluğu not et.

## Skill mi, Agent mı?

- **Skill** → tekrar kullanılabilir *bilgi/prosedür* (nasıl yapılır, kontrol listesi, referans).
  Progressive disclosure ile yüklenir; `SKILL.md` + `name`/`description` frontmatter.
- **Agent** → tools ile *otonom alt-persona* (analiz/orkestrasyon/uzman rol).
  `agents/<ad>.md` + frontmatter (`name`, `description`, `model`, `tools`).

## Nasıl ve nereye eklenir

Kütüphane **`mutfak-yazilimevi/.github` → `project-template/.claude/`** altındadır (tek kaynak).
İçeriği projeye kopyalama; merkeze ekle, sonra yayımla.

**Skill:** `project-template/.claude/skills/<önek>-<ad>/SKILL.md` — doğru **önek** plugin'i belirler:

| Önek | Plugin | | Önek | Plugin |
| :-- | :-- | :-- | :-- | :-- |
| `dev-dotnet-` | mutfak-dotnet | | `sec-` | mutfak-security |
| `dev-` | mutfak-dev | | `pm-` | mutfak-pm |
| `fe-` / `web-` | mutfak-frontend | | `mkt-` / `seo` | mutfak-marketing |
| `design-` | mutfak-design | | `md-` | mutfak-diagrams |
| `research-` | mutfak-research | | `ali-` | mutfak-consulting |
| (öneksiz) | mutfak-core | | | |

**Agent:** `project-template/.claude/agents/<ad>.md` — kurallar:
- `name` alanı **dosya adıyla birebir** aynı olmalı.
- `model`: mekanik/yapılandırılmış iş → `haiku`, muhakeme/mimari/uzmanlık → `sonnet`,
  en karmaşık orkestrasyon → `opus` (en az token + ortalama-üstü performans).
- `tools`: yalnız gerçek araç adları (`Read`, `Write`, `Edit`, `Bash`, `Grep`, … veya `mcp__*`);
  tekrarsız. Eklersen `agents/index_agents.md`'yi de güncelle.
- Var olmayan agent'a yönlendirme yazma; yalnız mevcut agent adlarını referansla.

## Yardımcı skill'ler (sıfırdan yazma)

Üretirken mevcut meta-skill'leri kullan:
- **Skill yazma:** `dev-skill-creator`, `dev-write-a-skill`, `dev-writing-skills`
- **Agent tasarımı:** `ali-agent-designer`
- **Plugin iskeleti:** `dev-plugin-creator`
- **.NET konuları:** önce `dev-aspnet-core` ve `dev-dotnet-*` skill'lerine bak — boşluk gerçekten var mı?

## Yayımla (tüm projelere ulaşsın)

`claude-config` reposunu kapsayan oturumda repo kökünde:

```bash
bash scripts/publish.sh   # .github'tan çek → 12 plugin'i materyalize et → commit + push
```

Proje tarafında yeni plugin gerekiyorsa `.claude/settings.json` → `enabledPlugins`'e ekle
(profiller için `templates/PROFILES.md`). Kütüphane kopyalanmaz, **referanslanır**.
