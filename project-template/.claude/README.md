# `.claude/` — Claude Code Yapılandırması & Kullanım Kılavuzu

Bu dizin, Claude Code'un proje kapsamındaki (project scope) yapılandırmasını tutar ve
yeni bir projeyi **merkezi skill & agent kütüphanesine** (plugin marketplace) bağlar.

Master plan: [`../CLAUDE-CODE-MASTERPLAN.md`](../CLAUDE-CODE-MASTERPLAN.md) · İlerleme: [`../TASK_LIST.md`](../TASK_LIST.md)
Marketplace: [`../../claude-config/README.md`](../../claude-config/README.md)

İçindekiler: [Yeni proje oluşturma](#yeni-proje-oluşturma-adım-adım) ·
[Skill kullanma](#skill-kullanma-kılavuzu) · [Agent kullanma](#agent-kullanma-kılavuzu) ·
[Skill mi agent mı?](#skill-mi-agent-mı) · [İndeksler](#i̇ndeksler-keşif) ·
[Dizin yapısı](#dizin-yapısı) · [Skill keşif kuralı](#skill-keşif-kuralı-kritik)

---

## Yeni proje oluşturma (adım adım)

Felsefe: **kütüphane kopyalanmaz, referanslanır.** Her proje yalnız küçük bir `.claude/settings.json`
taşır; gerçek skill/agent içeriği marketplace'ten çekilir → en az token, ortalama-üstü performans.

### 1. Bu şablonu projeye al

`project-template` iskeletini yeni reponun temeli yap. En az gerekenler:

```
.claude/      → yapılandırma (bu dizin)
CLAUDE.md     → takım talimatı (mimari + kurallar)
.gitignore    → CLAUDE.local.md, materyalize içerik vb. hariç tutar
.mcp.json     → gerekli MCP sunucuları
```

`docs/`, `_catalog/`, `_staging/` opsiyoneldir (referans / çalışma alanı; auto-load edilmez).

### 2. Marketplace'i ekle ve plugin etkinleştir

Claude Code oturumu içinde:

```text
/plugin marketplace add mutfak-yazilimevi/claude-config
/plugin install mutfak-dotnet@mutfak
/plugin install mutfak-security@mutfak
```

…veya projeye kalıcı yazmak için `.claude/settings.json`:

```json
{
  "extraKnownMarketplaces": {
    "mutfak": { "source": { "source": "github", "repo": "mutfak-yazilimevi/claude-config" } }
  },
  "enabledPlugins": { "mutfak-dotnet@mutfak": true, "mutfak-security@mutfak": true }
}
```

> `enabledPlugins` **object** formatındadır (`"plugin@market": true`), array değil.
> Mevcut bir projeye eklerken elle yazmak yerine güvenli merge script'ini kullanabilirsin:
> `claude-config/scripts/integrate-into-project.sh --target . --profile dotnet-api`.
>
> Yalnız **gerekli** plugin'leri aç (minimum token). Proje tipine göre hazır profiller için
> marketplace reposundaki `templates/PROFILES.md`'ye bak. 12 plugin ve içerikleri:
> [`claude-config/README.md` → Plugin'ler](../../claude-config/README.md#pluginler).

### 3. `CLAUDE.md`'yi projeye göre doldur

Kök [`../CLAUDE.md`](../CLAUDE.md) takım talimatıdır (commit edilir). `<...>` alanlarını doldur,
dosyayı **<200 satır** tut. Kişisel/geçici notlar `CLAUDE.local.md`'ye (gitignore).

### 4. Kuralları ve komutları gözden geçir

- `rules/` — `code-style.md`, `testing.md`, `api-conventions.md`, `capability-gaps.md`
- `commands/` — slash komutları (`/review`, `/fix-issue`, `/deploy`)
- `.mcp.json` — projenin ihtiyaç duyduğu MCP sunucuları

### 5. Çalıştır

Proje açıldığında Claude Code plugin'leri marketplace'ten çeker; skill ve agent'lar otomatik
kullanılabilir olur. Günlük kullanım için aşağıdaki iki kılavuza geç.

---

## Skill kullanma kılavuzu

**Skill = prosedür/bilgi paketi.** Belirli bir işi *nasıl* yapacağını anlatan, talimat + opsiyonel
script/şablon içeren bir `SKILL.md` birimidir (örn. `dev-tdd`, `sec-analyzing-network-traffic-with-wireshark`, `mkt-seo-audit`).

### Nasıl tetiklenir?

1. **Otomatik (önerilen):** Claude, etkin plugin'lerdeki skill'lerin `description`'ını göreve göre
   eşler ve uygun olanı kendiliğinden devreye sokar. Sen sadece işi tarif edersin:
   > "Bu endpoint'i TDD ile yapalım." → `dev-tdd` devreye girer.
   > "Şu PCAP'te C2 trafiği var mı?" → ilgili `sec-analyzing-...` skill'i kullanılır.
2. **Açık çağrı:** İstediğin skill'i adıyla iste:
   > "`dev-systematic-debugging` skill'ini kullanarak ilerle."
3. **Slash:** Kullanıcı-çağrılabilir skill'ler `/` menüsünde plugin adıyla görünür
   (örn. `/mutfak-dev:dev-tdd`). `/` yazıp listeden seçebilirsin.

### İyi sonuç için ipuçları

- **Bağlamı ver, skill'i değil:** Hangi skill olduğunu bilmesen de görevi net tarif et; eşleşme
  `description` üzerinden olur. Yanlış skill seçildiyse "X yerine Y skill'ini kullan" de.
- **Keşif:** Hangi skill'ler var? → [`skills/index_skills.md`](skills/index_skills.md) (1606 skill,
  plugin → alt-kategori, ad-only). Detay için ilgili `skills/<ad>/SKILL.md`.
- **Önek = alan:** `dev-` geliştirme · `sec-` güvenlik · `mkt-`/`seo` pazarlama · `pm-` ürün ·
  `design-` tasarım · `ali-` danışmanlık · `md-` diyagram · `research-` araştırma · `fe-`/`web-` frontend.

---

## Agent kullanma kılavuzu

**Agent (sub-agent) = otonom uzman persona.** Kendi sistem talimatı ve araçları olan, çok adımlı bir işi
baştan sona yürüten uzmandır (örn. `backend-architect`, `code-reviewer-pro`, `security-auditor`,
`tech-lead-orchestrator`). Skill bir *yöntem*tir; agent o yöntemleri kullanan bir *kişi* gibidir ve
**kendi izole bağlamında** çalışır (ana konuşmayı şişirmez).

### Nasıl çağrılır?

1. **Otomatik delegasyon:** Claude, görev bir uzmana uygunsa (özellikle çok adımlı/araştırma
   gerektiren işlerde) işi ilgili agent'a devreder.
2. **Açık çağrı:** Agent'ı adıyla iste:
   > "`code-reviewer-pro` ile bu diff'i incele."
   > "Bunu `tech-lead-orchestrator`'a devret, alt görevlere bölsün."
3. **Orkestrasyon:** Büyük/çok parçalı işlerde önce bir orkestratör kullan
   (`tech-lead-orchestrator`, `agent-organizer`) veya `mutfak-spec-workflow` spec zinciri:
   `spec-analyst → spec-architect → spec-planner → spec-developer → spec-tester → spec-reviewer`.

### İyi sonuç için ipuçları

- **Paralel iş = paralel agent:** Bağımsız alt görevleri ayrı agent'lara aynı anda dağıt.
- **Doğru uzmanı seç:** mimari karar → `*-architect` · hata ayıklama → `debugger` · inceleme →
  `code-reviewer-pro` · güvenlik → `security-auditor` · tanıdık olmayan kod tabanı → `project-analyst`.
- **Keşif:** Hangi agent'lar var? → [`agents/index_agents.md`](agents/index_agents.md)
  (64 agent, 12 kategori, model + açıklama).

---

## Skill mi, agent mı?

| | **Skill** | **Agent** |
| :--- | :--- | :--- |
| Ne | Prosedür / bilgi (nasıl yapılır) | Otonom uzman persona (kim yapar) |
| Kapsam | Tek, odaklı yetenek | Çok adımlı görev, karar verir |
| Bağlam | Ana konuşmaya yüklenir | Kendi izole bağlamında çalışır |
| Örnek | `dev-tdd`, `mkt-seo-audit` | `backend-architect`, `security-auditor` |
| Ne zaman | Belirli bir işi doğru yöntemle yap | Bir alanı uzmana devret / paralelleştir |

Pratik kural: **dar ve tek seferlik** iş → skill. **Geniş, çok adımlı veya delege edilebilir** iş → agent.
Agent'lar zaten ilgili skill'leri içeride kullanır.

---

## İndeksler (keşif)

| İndeks | Kapsam | Bakım |
| :--- | :--- | :--- |
| [`skills/index_skills.md`](skills/index_skills.md) | 1606 skill, plugin → alt-kategori (ad-only) | `claude-config/scripts/build-skill-index.py` ile **üretilir** |
| [`agents/index_agents.md`](agents/index_agents.md) | 64 agent, 12 kategori (model + açıklama) | **elle** tutulur |

> Skill indeksini elle düzenleme; `--source <.claude>` ile yeniden üret. Yeni agent eklerken
> `index_agents.md`'yi elle güncelle. Yeni yetenek üretme kuralları: [`rules/capability-gaps.md`](rules/capability-gaps.md).

---

## Dizin yapısı

| Klasör / Dosya | Amaç |
| :--- | :--- |
| `settings.json` | İzinler + hook'lar + config (commit edilir, takım) |
| `settings.local.json` | Kişisel override'lar (gitignore) — örnek: `settings.local.json.example` |
| `rules/` | Modüler kurallar: `code-style`, `testing`, `api-conventions`, `architecture`, `scaling`, `mcp`, `process` (SDLC/ADLC), `capability-gaps` |
| `commands/` | Slash komutları — `/intake` (yeni proje), `/onboard` (mevcut proje, salt-okunur), `/review`, `/fix-issue`, `/deploy`, `/test-all`, `/bootstrap`, `/document`, `/refactor` |
| `skills/` | ⚠️ **DÜZ yapı** — her skill `skills/<önek-ad>/SKILL.md`; iç içe kategori klasörü otomatik bulunmaz |
| `agents/` | Sub-agent tanımları (`<ad>.md`, düz) |
| `hooks/` | Guardrail/otomasyon (taksonomi: `hooks/README.md`) — Pre/PostToolUse, SessionStart/End, PreCompact (sır taraması), Notification/Stop |
| `.mcp.json` (kök) | MCP sunucuları: context7, sequential-thinking, github, postgres, playwright (bkz. `rules/mcp.md`) |
| `.env.example` (kök) | Ortam değişkeni şablonu (`.env` olarak kopyala; `.env*` gitignore) |
| `.worktreeinclude` (kök) | git worktree'lere taşınacak yerel/gitignored dosyalar |
| `output-styles/` | Yanıt stilleri (ton/format) — opsiyonel, `/output-style <ad>` |
| `agent-memory/` | Sub-agent'ların oturumlar arası kalıcı hafızası |

> Tek bir projede `skills/`/`agents/` içeriği genelde marketplace plugin'lerinden gelir; bu klasörler
> **proje-özel** skill/agent eklemek istersen kullanılır. Tekrar kullanılabilir bir boşluk çıkarsa
> tek seferlik çözmek yerine merkezi kütüphaneye eklemeyi öner → [`rules/capability-gaps.md`](rules/capability-gaps.md).

### Kapsam: proje vs global (`~/.claude/`)

İki `.claude/` vardır: **proje** (bu dizin — takımla paylaşılır, git'e commit edilir) ve **global**
(`~/.claude/` — senin makinen, tüm projeler). Memory hiyerarşisi:
`global → monorepo kök → proje (./CLAUDE.md) → alt-klasör`; alt bağlam üstü **ezmez, tamamlar**.
Kişisel tercihleri global'e veya `*.local.*` dosyalarına; takım kurallarını bu dizine koy.

**Proje scope (bu repo — commit edilir):** `CLAUDE.md` · `.mcp.json` · `.worktreeinclude` ·
`.env.example` · `.claude/{settings.json, rules/, commands/, skills/, agents/, hooks/,
output-styles/, agent-memory/}`. Kişisel/gitignore: `CLAUDE.local.md`,
`.claude/settings.local.json` (örnek: `settings.local.json.example`).

**Global scope (makinende, repo'da DEĞİL):** `~/.claude/{CLAUDE.md, settings.json, commands/,
skills/, agents/, agent-memory/, plugins/, projects/}` + `~/.claude.json`, `keybindings.json`.
Bunlar developer-başınadır; template'e dahil edilmez (doğru olan budur). `~/.claude.json` ve
`plugins/` **silinmez**.

## Skill keşif kuralı (kritik)

Claude Code skill'leri **yalnızca** `.claude/skills/<ad>/SKILL.md` yolundan tarar.
Gruplama için **ad öneki** kullanılır: `dev-`, `fe-`, `seo-`, `mkt-`, `pm-`, `design-`, `research-`, `sec-`.

Alt-klasörler **namespace sağlamaz** — çağrı adı yalnız `SKILL.md`/agent frontmatter'ındaki `name`'den
gelir; dosya yolu yok sayılır. `_staging/`, `_catalog/` ve `docs/` **asla** `.claude/skills/` altında
olmaz (auto-load edilmez).
