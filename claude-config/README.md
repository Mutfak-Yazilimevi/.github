# Mutfak · Claude Code Plugin Marketplace

> Organizasyonun **tüm Claude Code skill & agent'larının** tek deposu.
> Projeler bu kütüphaneyi **kopyalamaz** — yalnızca ihtiyaç duyduğu plugin'leri **açar.**

**📦 12 plugin · 🧩 1613 skill · 🤖 64 agent · ⌨️ 9 slash komutu**

---

## ⚠️ Marketplace ne taşır / ne taşımaz?

Bu en kritik ayrım — **marketplace = kütüphane**, tam proje kurulumu değildir.

| Plugin (marketplace) **taşır** | Plugin **taşımaz** (proje scaffold'u) |
| :--- | :--- |
| ✅ **skills/** (1613) | ❌ `rules/` (capability-gaps, process, catalog, model-selection…) |
| ✅ **agents/** (64) | ❌ `CLAUDE.md` (proje konvansiyonları) |
| ✅ **commands/** → `/mutfak-onboard`, `/mutfak-intake`, `/mutfak-review` … (`mutfak-core`) | ❌ `hooks/` · `.mcp.json` |

- **Kurallar ve CLAUDE.md plugin kavramı değildir** — bunlar projenin bağlamıdır, marketplace ile gelmez.
- **Tam kurulum** (kurallar + hooks + CLAUDE.md dahil) için: `project-template`'i temel al veya `integrate-into-project.sh` ile entegre et.
- `/mutfak-onboard`, `/mutfak-intake` komutları **`mutfak-core`** ile gelir → `enabledPlugins`'e **`mutfak-core`'u eklemeyi unutma.**

---

## 30 saniyede ne işe yarar?

Bunu **ortak bir mutfak deposu** gibi düşün: bütün malzemeler (skill ve agent'lar) tek yerde
durur. Her projeye dolabı baştan kurmak yerine, proje sadece **gereken rafları açar.**

- ✅ **Kopya yok** — proje yalnızca küçük bir `settings.json` taşır.
- ✅ **Az token** — yalnız ilgili plugin yüklenir (progressive disclosure).
- ✅ **Tek kaynak** — kütüphane bir kez burada güncellenir, **tüm projeler** faydalanır.

---

## Hangisini yapmak istiyorsun?

| Amacın | Git → |
| :--- | :--- |
| 🧑‍💻 **Bir projede kullanmak** istiyorum | [1. Projede Kullanım](#1-projede-kullanım) |
| 📦 **İçinde ne var** görmek istiyorum | [2. Plugin'ler](#2-pluginler) |
| 🛠️ **Kütüphaneyi yönetmek** (büyütmek/yayımlamak) istiyorum | [3. Bakım & Yayımlama](#3-bakım--yayımlama) |

---

## 1. Projede Kullanım

### En kolay yol — tek komut
Mevcut bir projeye marketplace'i güvenle ekler (proje `.claude/` olsa da olmasa da; hiçbir şeyi ezmez):

```bash
bash scripts/integrate-into-project.sh --target /yol/projeye --profile dotnet-api
```

### Elle yapmak istersen — `.claude/settings.json`
```json
{
  "extraKnownMarketplaces": {
    "mutfak": { "source": { "source": "github", "repo": "mutfak-yazilimevi/claude-config" } }
  },
  "enabledPlugins": { "mutfak-dotnet@mutfak": true, "mutfak-security@mutfak": true }
}
```
> `enabledPlugins` **object** formatındadır (`"plugin@mutfak": true`), array değil.

### Sonra ne olacak? (doğrulama)

`settings.json`'ı koydun — sıradaki adımlar:

1. **Claude Code'u proje kökünde (yeniden) aç.** Ayarlar oturum **başında** okunur; açıksa oturumu yenile.
2. **İlk açılışta marketplace'e "güven" sorulabilir** → onayla. Claude Code `enabledPlugins`'teki plugin'leri marketplace'ten çeker.
3. **Doğrula:** `/plugin` yaz → plugin'ler "enabled" görünmeli; `/` menüsünde komut/skill'ler gelir.
4. **Yüklenmezse elle tetikle:**
   ```text
   /plugin marketplace add mutfak-yazilimevi/claude-config
   /plugin install mutfak-dotnet@mutfak
   ```
5. **`CLAUDE.md` ekle/doldur** (mimari + kurallar). `integrate-into-project.sh` kullandıysan bir stub bırakılmıştır.
6. **Çalışmaya başla:** işi tarif et (skill'ler otomatik tetiklenir) — yeni projede `/mutfak-intake`, mevcut projede `/mutfak-onboard`.

> **Önemli — sık yaşanan kafa karışıklıkları:**
> - `/plugin` **yalnız terminal CLI ve IDE eklentilerinde** vardır — **claude.ai web'de yoktur.** "isn't available" diyorsa: web'desin veya sürümün eski (`npm i -g @anthropic-ai/claude-code@latest`).
> - `/plugin marketplace add` interaktif bir komuttur; önce **giriş** gerekir (`/login`). `claude /plugin …` diye kabuktan çalıştırılmaz.
> - Plugin'ler projenin `.claude/`'ına **kopyalanmaz** — `~/.claude/plugins/`'e (global) iner. Projede bir şey görmemen **normaldir.**
> - `/mutfak-onboard`, `/mutfak-intake` görünmüyorsa: `mutfak-core` etkin mi? (Komutlar onunla gelir.)

### Hazır profiller (`--profile`)
Proje tipine göre önerilen plugin setleri — detay: [`templates/PROFILES.md`](templates/PROFILES.md).

| Profil | Açılan plugin'ler |
| :--- | :--- |
| `dotnet-api` | core · dev · dotnet · security |
| `web-frontend` | core · dev · frontend · design |
| `fullstack` | core · dev · dotnet · frontend · design · security |
| `marketing` | core · marketing · pm · design |
| `security` | core · dev · security |
| `minimal` | core · dev |

> Tam liste: `minimal, dotnet-api, web-frontend, fullstack, mobile, marketing, product, security, research, spec`.

---

## 2. Plugin'ler

| Plugin | İçinde ne var |
| :--- | :--- |
| `mutfak-core` | Genel/çekirdek skill'ler + genel agent'lar **+ slash komutları** (`/mutfak-onboard`, `/mutfak-intake`, `/mutfak-review`, `/mutfak-deploy`…) |
| `mutfak-dev` | Genel geliştirme: TDD, review, debug, mimari, deploy, dil uzmanları |
| `mutfak-dotnet` | .NET skill'leri + backend/Akka.NET/Roslyn/performans agent'ları |
| `mutfak-frontend` | Premium UI / UX / React / Next + frontend agent'ları |
| `mutfak-design` | Vendor marka/ürün tasarım sistemleri (99+) |
| `mutfak-pm` | Ürün yönetimi: strateji, PRD, metrik, keşif |
| `mutfak-marketing` | Pazarlama, SEO, growth, içerik + blog agent'ları |
| `mutfak-security` | Savunma/blue-team güvenlik (forensics, detection, hunting…) |
| `mutfak-research` | Akademik & derin araştırma |
| `mutfak-diagrams` | Markdown diyagram/görselleştirme |
| `mutfak-consulting` | İş/ops/C-level danışmanlık |
| `mutfak-spec-workflow` | Spec-driven orkestrasyon (spec-analyst → … → spec-validator) |

**Ne var, nerede?** Hızlı arama için kataloglar:
- 🔎 Skill'ler: `…/skills/skills-catalog.csv` (ad · kategori · teknoloji · yol · açıklama)
- 🔎 Agent'lar: `…/agents/agents-catalog.csv`
- 📖 İnsan-okur indeksler: `index_skills.md`, `index_agents.md`

---

## 3. Bakım & Yayımlama

Yeni skill/agent ekleyince izlenecek akış:

```
1) ekle  →  2) indeks/katalog üret  →  3) denetle  →  4) yayımla
```

### Script'ler (hepsi `scripts/` altında)

| Script | Ne yapar |
| :--- | :--- |
| `build-marketplace.sh` | Skill/agent'ları önek bazlı 12 plugin'e **materyalize** eder |
| `build-skill-index.py` | `index_skills.md` (kategori bazlı ad listesi) üretir |
| `build-catalog.py` | Sorgulanabilir CSV kataloglarını (skills/agents) üretir |
| `audit-claude.py` | Kütüphaneyi denetler (frontmatter, çakışma, kırık referans, senkron) — ERR'de exit 1 |
| `integrate-into-project.sh` | Marketplace'i mevcut bir projeye güvenle ekler |
| `publish.sh` | `.github`'tan çekip 12 plugin'i materyalize eder, commit + push (**tek komut yayım**) |

Her biri `--source <.claude dizini>` alır (publish hariç). Örn:
```bash
python3 scripts/build-catalog.py --source ../project-template/.claude
python3 scripts/audit-claude.py  --source ../project-template/.claude --repo-root ../project-template
```

### Yayımla (tüm projelere ulaşsın)
```bash
bash scripts/publish.sh
```

---

## Nasıl çalışıyor? (zihinsel model)

- **Kaynak tek yerde:** asıl skill/agent içeriği `mutfak-yazilimevi/.github/project-template/.claude/` altındadır.
- **Önek = plugin:** skill adının öneki hangi plugin'e gideceğini belirler (`dev-` → mutfak-dev, `sec-` → mutfak-security…). Agent'lar ise ada göre eşlenir.
- **Materyalize:** `build-marketplace.sh` skill/agent'ları `plugins/<ad>/{skills,agents}/`, **slash komutlarını** `plugins/mutfak-core/commands/` altına kopyalar.
- **Plugin ile gelmeyenler:** `rules/`, `CLAUDE.md`, `hooks/`, `.mcp.json` — proje scaffold'udur (bkz. "Marketplace ne taşır/taşımaz").
- **Proje tarafı:** proje yalnız `settings.json` ile plugin'i **açar**; içeriği Claude Code marketplace'ten çeker (`~/.claude/plugins/`).

---

## Dizin yapısı

```
claude-config/
├── marketplace.json        # marketplace tanımı
├── plugins/                # 12 plugin (materyalize skill/agent içeriği)
├── scripts/                # build / catalog / audit / integrate / publish
├── templates/              # proje settings.json + PROFILES.md
└── README.md               # bu dosya
```

> Yeni yetenek boşluğu çıktığında tek seferlik çözmek yerine kütüphaneye eklenir →
> bkz. `project-template/.claude/rules/capability-gaps.md`.
