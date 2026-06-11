# Claude Code — Skill & Sub-Agent Kütüphanesi: Eksiksiz Master Plan

> **Amaç:** 36 skill reposu + 8 sub-agent reposunu, görsellerdeki kanonik mimariye göre, Claude Code'da kurulabilir tek bir yapıda toplamak.
> **Kullanım:** Bu dosyayı Claude Code projenin köküne koy, fazları sırayla çalıştır. Tüm linkler, komutlar ve kararlar dahildir.
> **Dayanak:** 31 infografik (mimari) + senin .NET ağırlıklı odağın.

---

## İÇİNDEKİLER
1. Tasarım ilkeleri (görsellerden)
2. Hedef klasör yapısı + skill keşif kuralı (kritik)
3. Skill repo master tablosu (36) — tam linkler + komutlar
4. Sub-agent repo master tablosu (8) — tam linkler + komutlar
5. Dedupe matrisleri (frontend / SEO-marketing / agent / katalog)
6. Lisans & güvenlik analizi
7. Aşamalı kurulum (Faz 0–7) — Claude Code komutlarıyla
8. Bootstrap dosyaları (CLAUDE.md, settings.json, .gitignore)
9. Doğrulama & bakım
10. Hızlı öncelik özeti

---

## 1. Tasarım ilkeleri (görsellerden)

| İlke | Kaynak | Özet |
|------|--------|------|
| 5 katmanlı Agent Development Kit | görsel 15, 20 | CLAUDE.md (Memory) → Skills (Knowledge) → Hooks (Guardrail) → Subagents (Delegation) → Plugins (Distribution) |
| İki scope | görsel 18 | Project (`./.claude/`, commit) + Global (`~/.claude/`, kişisel) |
| SKILL.md Universal Standard + Progressive Disclosure | görsel 30 | L1 name+desc (auto-trigger) · L2 gövde <5.000 token · L3 scripts/refs on-demand · maks 500 satır |
| Memory hiyerarşisi | görsel 9, 23 | global → monorepo kök → proje → alt-klasör; her dosya <200 satır, üstü ezmez |
| Context yönetimi | görsel 8, 9 | %50-70 izle → %70-90 `/compact` → %90+ `/clear` |
| 4 katman zihinsel model | görsel 9 | L1 CLAUDE.md (context) · L2 Skills (bilgi) · L3 Hooks (güvenlik) · L4 Agents (delegasyon) |

---

## 2. Hedef klasör yapısı + skill keşif kuralı (KRİTİK)

### 2.1 Yapı
```
your-project/
├── CLAUDE.md                 # takım talimatı (commit) — Bölüm 8.1
├── CLAUDE.local.md           # kişisel (gitignore)
├── .mcp.json                 # takım MCP sunucuları (commit)
├── .gitignore                # Bölüm 8.3
├── .claude/
│   ├── settings.json         # izin + hook + env (commit) — Bölüm 8.2
│   ├── settings.local.json   # kişisel (gitignore)
│   ├── rules/                # code-style.md · testing.md · api-conventions.md
│   ├── commands/             # review.md · fix-issue.md · deploy.md
│   ├── skills/               # ⚠️ DÜZ — her skill: skills/<ad>/SKILL.md
│   ├── agents/               # <ad>.md (düz)
│   └── hooks/                # validate-bash.sh
├── _staging/                 # ⛔ .claude DIŞINDA — lisanssız/koşullu (auto-load EDİLMEZ)
├── _catalog/                 # ⛔ .claude DIŞINDA — mega-katalog index (import değil)
└── docs/
    └── references/           # skill-olmayan referanslar (app-ideas, system-prompts)
```

### 2.2 Skill keşif kuralı (yanlış yapılırsa skill'ler görünmez)
Claude Code skill'leri **yalnızca** `.claude/skills/<ad>/SKILL.md` yolundan tarar. **İç içe kategori klasörü** (`skills/dev/...`) otomatik bulunmaz.

**Çözüm — ad öneki ile grupla (uygulanacak yöntem):**
```
dev-tdd/  dev-code-review/  fe-premium-ui/  fe-uiux-audit/  fe-generative-ui/
seo-technical/  mkt-copywriting/  pm-lean-canvas/  design-baseline-ui/
research-paper-reviewer/  sec-soc-playbook/
```
- Ölçeklenince domain kümelerini **plugin** olarak paketle (görsel 15/20).
- `_staging/`, `_catalog/`, `docs/` **asla** `.claude/skills/` altında olmaz.

---

## 3. Skill repo master tablosu (36)

**Durum:** ✅ ekle/eklendi · ⏳ işlendi · ⚠️ koşullu/seçmeli · ❌ alma · 📚 katalog
**Önek:** her skill `.claude/skills/<önek-ad>/` olarak yerleşir.

| # | Repo + Link | Sayı | Lisans | Karar | Hedef / Önek |
|---|-------------|------|--------|-------|--------------|
| 1 | [anthropics/skills](https://github.com/anthropics/skills/tree/main) | 17 | — | ✅ eklendi | skills/ |
| 2 | [keenthemes/reui](https://github.com/keenthemes/reui/tree/main) | 6 | — | ✅ eklendi | skills/ `design-` |
| 3 | [mattpocock/skills](https://github.com/mattpocock/skills/tree/main/skills) | — | — | ⏳ işlendi | skills/ `dev-` |
| 4 | [openai/skills](https://github.com/openai/skills/tree/main) | — | — | ⏳ işlendi | skills/ `dev-` |
| 5 | [MiniMax-AI/skills](https://github.com/MiniMax-AI/skills/tree/main) | — | — | ⏳ işlendi | skills/ `dev-` |
| 6 | [obra/superpowers](https://github.com/obra/superpowers/tree/main/skills) | — | — | ⏳ işlendi | skills/ `dev-` |
| 7 | [alirezarezvani/claude-skills](https://github.com/alirezarezvani/claude-skills/tree/main) | — | — | ⏳ işlendi | skills/ `mkt-` (dedupe) |
| 8 | [cloudflare/skills](https://github.com/cloudflare/skills/tree/main/skills) | 9 | — | ⏳ işlendi | skills/ `dev-` |
| 9 | [HighMark-31/TRAE-Skills](https://github.com/HighMark-31/TRAE-Skills/tree/main) | — | kısıtlı | ❌ alınmadı | — |
| 10 | [ihlamury/design-skills](https://github.com/ihlamury/design-skills/tree/main/skills) | 87 | **YOK** | ⚠️ seçmeli | `_staging/` |
| 11 | [mikeoptimax/claude-frontend-skills-pack](https://github.com/mikeoptimax/claude-frontend-skills-pack/tree/main) | 0 (CLAUDE.md) | MIT | ⚠️ şablon | `rules/` ref |
| 12 | [ziguishian/premium-ui-builder-skill](https://github.com/ziguishian/premium-ui-builder-skill/tree/main) | 1 | MIT | ✅ ekle | skills/ `fe-premium-ui` |
| 13 | [Stevenshanmukh/Claude_UI-UX-Design-Consultant-Skill](https://github.com/Stevenshanmukh/Claude_UI-UX-Design-Consultant-Skill/tree/master) | 1 | MIT | ✅ ekle | skills/ `fe-uiux-audit` |
| 14 | [metawhisp/amazing-seo-skill](https://github.com/metawhisp/amazing-seo-skill/tree/main) | 1 | Apache-2.0 | ✅ çekirdek, izole | skills/ `seo-technical` |
| 15 | [addyosmani/agent-skills](https://github.com/addyosmani/agent-skills/tree/main) | 24 | MIT | ✅ **yüksek değer** | skills/ `dev-` |
| 16 | [dotnet/skills](https://github.com/dotnet/skills/tree/main) | 100 | MIT | ✅ **yüksek değer, seçmeli** | skills/ `dev-dotnet-` |
| 17 | [microsoft/skills](https://github.com/microsoft/skills/tree/main) | 189 | MIT | ✅ seçmeli | skills/ `dev-` (skill-creator, mcp-builder) |
| 18 | [ai-freer/generative-ui-skill](https://github.com/ai-freer/generative-ui-skill/tree/main) | 1 | Apache-2.0 | ✅ (Chrome CDP) | skills/ `fe-generative-ui` |
| 19 | [Ilm-Alan/frontend-design](https://github.com/Ilm-Alan/frontend-design/tree/main) | 1 | MIT | ⚠️ **isim çakışması** | yeniden adlandır / ele |
| 20 | [deveshpunjabi/modern-frontend-skill](https://github.com/deveshpunjabi/modern-frontend-skill/tree/main) | 1 | MIT | ⚠️ çakışma | #12 ile seç |
| 21 | [master5d/claude-design-skills](https://github.com/master5d/claude-design-skills/tree/master) | 6 | **YOK** | ⚠️ koşullu | `_staging/` |
| 22 | [slavingia/skills](https://github.com/slavingia/skills/tree/main) | 10 | **YOK** | ⚠️ koşullu | `_staging/` `pm-` |
| 23 | [remotion-dev/skills](https://github.com/remotion-dev/skills/tree/main/skills) | 1 | belirsiz | ⚠️ niş | `_staging/` |
| 24 | [markdown-viewer/skills](https://github.com/markdown-viewer/skills/tree/main) | 15 | **YOK** | ⚠️ koşullu (işine yakın) | `_staging/` |
| 25 | [ComposioHQ/awesome-claude-skills](https://github.com/ComposioHQ/awesome-claude-skills/tree/master) | 864 | yok | 📚 katalog | `_catalog/` |
| 26 | [sickn33/antigravity-awesome-skills](https://github.com/sickn33/antigravity-awesome-skills/tree/main/skills) | 1.527+ | CC BY 4.0+MIT | 📚 katalog | `_catalog/` |
| 27 | [phuryn/pm-skills](https://github.com/phuryn/pm-skills/tree/main) | 68 | MIT | ✅ ekle | skills/ `pm-` |
| 28 | [florinpop17/app-ideas](https://github.com/florinpop17/app-ideas/tree/master) | 0 | MIT | ❌ **skill değil** | `docs/references/` |
| 29 | [K-Dense-AI/scientific-agent-skills](https://github.com/K-Dense-AI/scientific-agent-skills/tree/main/skills) | 144 | MIT | ⚠️ seçmeli (niş) | skills/ `research-` (gerekirse) |
| 30 | [Imbad0202/academic-research-skills](https://github.com/Imbad0202/academic-research-skills/tree/main) | 4 | CC | ✅ küçük (atıf) | skills/ `research-` |
| 31 | [mukul975/Anthropic-Cybersecurity-Skills](https://github.com/mukul975/Anthropic-Cybersecurity-Skills/tree/main) | 754 | Apache-2.0 | ⚠️ **savunma alt kümesi; resmî DEĞİL** | skills/ `sec-` (blue-team) |
| 32 | [coreyhaines31/marketingskills](https://github.com/coreyhaines31/marketingskills/tree/main) | 44 | MIT | ✅ ekle | skills/ `mkt-` |
| 33 | [ComposioHQ/awesome-codex-skills](https://github.com/ComposioHQ/awesome-codex-skills/tree/master) | 880 | yok | 📚 katalog (#25 ~aynı) | `_catalog/` |
| 34 | [ConardLi/garden-skills](https://github.com/ConardLi/garden-skills/tree/main/skills) | 5 | MIT | ✅ küçük | skills/ |
| 35 | [github/awesome-copilot](https://github.com/github/awesome-copilot/tree/main/skills) | 521 | MIT | ⚠️ seçmeli (karışık kalite) | skills/ `dev-` (seç) |
| 36 | [ericosiu/ai-marketing-skills](https://github.com/ericosiu/ai-marketing-skills/tree/main) | 22 | MIT | ✅ ekle | skills/ `mkt-` |

### 3.1 Genel ingest reçetesi
- **Repo `skills/<ad>/SKILL.md` yapısındaysa** (3,6,8,10,16,17,22,23,24,29,31,32,34,35): istenen `<ad>` klasörlerini `.claude/skills/<önek-ad>/` olarak kopyala.
- **Repo kökünde tek SKILL.md** (12,13,14,18,19,20,30): repo'yu bir klasör olarak `.claude/skills/<önek-ad>/` altına koy.
- **CLAUDE.md şablonu** (11): `rules/`'a referans olarak koy, skills'e değil.

---

## 4. Sub-agent repo master tablosu (8)

Hedef: `.claude/agents/<ad>.md` (düz). Görsel 15 kuralı: subagent subagent doğuramaz; orchestrator'lar ana context'te `Task` ile koordine eder.

| # | Repo + Link | Agent | Lisans | Karar |
|---|-------------|-------|--------|-------|
| SA1 | [vijaythecoder/awesome-claude-agents](https://github.com/vijaythecoder/awesome-claude-agents/tree/main/agents) | 33 | MIT | ✅ seçmeli |
| SA2 | [lst97/claude-code-sub-agents](https://github.com/lst97/claude-code-sub-agents/tree/main/agents) | 37 | MIT | ✅ **yüksek değer** |
| SA3 | [hesreallyhim/a-list-of-claude-code-agents](https://github.com/hesreallyhim/a-list-of-claude-code-agents/tree/main/agents) | 4 (index) | YOK | 📚 katalog |
| SA4 | [Aaronontheweb/dotnet-skills](https://github.com/Aaronontheweb/dotnet-skills/tree/master/agents) | 6 | MIT | ✅ **.NET gold** |
| SA5 | [zhsama/claude-sub-agent](https://github.com/zhsama/claude-sub-agent/tree/main/agents) | 12 | YOK | ⚠️ koşullu |
| SA6 | [AgriciDaniel/claude-seo](https://github.com/AgriciDaniel/claude-seo/tree/main/agents) | 1 | MIT | ⚠️ minimal |
| SA7 | [Piebald-AI/claude-code-system-prompts](https://github.com/Piebald-AI/claude-code-system-prompts/tree/main) | 0 (352 prompt) | MIT | ❌ **agent değil** |
| SA8 | [AgriciDaniel/claude-blog](https://github.com/AgriciDaniel/claude-blog/tree/main/agents) | 5 | MIT | ✅ küçük |

### 4.1 Agent analizi
- **SA2 (37, MIT) — birincil genel set.** backend-architect, typescript/react/python/golang-pro, cloud-architect, data-engineer, ml-engineer, postgres-pro, database-optimizer, graphql-architect, ux/ui-designer, product-manager, prompt-engineer, agent-organizer. Frontmatter'da MCP araçları (`context7`, `sequential-thinking`) + `model` tanımlı → **bu MCP'leri kur ya da araç referanslarını temizle**, yoksa agent hata verir.
- **SA4 (6, MIT) — .NET'e birebir.** akka-net-specialist, roslyn-incremental-generator-specialist, dotnet-concurrency-specialist, dotnet-performance-analyst, dotnet-benchmark-designer, docfx-specialist. (Repo adı `dotnet-skills` ama resmî `dotnet/skills` #16'dan farklı — Aaron Stannard/Akka.NET.)
- **SA1 (33, MIT) — SA2 ile büyük örtüşme.** Sadece **orchestrator'ları** al: `tech-lead-orchestrator`, `team-configurator`, `project-analyst`. Laravel/Rails/Django gibi .NET-dışı framework uzmanlarını ele.
- **SA8 (5, MIT)** — blog pipeline: writer/seo/translator/researcher/reviewer. Küçük, temiz.
- **SA5 (12, lisans YOK)** — bütünleşik **spec-driven workflow** sistemi (spec-orchestrator/planner/architect/developer/tester/reviewer/validator/analyst). Değerli ama lisans yok → `_staging/`.
- **SA6 (1)** — yalnız `seo-image-gen`; agent olarak minimal.
- **SA3 (index)** — "a-list-of" awesome listesi → `_catalog/`, kaynak değil.
- **SA7 (352 prompt)** — agent değil; Claude Code'un **iç sistem-prompt'larının** koleksiyonu (tool description'lar, agent prompt'ları). İçerik Anthropic'e ait → **IP-hassas**, ana yapıya katma; istersen yalnız `docs/references/` altında inceleme amaçlı.

---

## 5. Dedupe matrisleri

### 5.1 Frontend (8 kaynak)
| Skill | Rol | Karar |
|-------|-----|-------|
| yerleşik `frontend-design` | sıfırdan görsel yön | tut |
| #12 premium-ui | sıfırdan premium sistem | **birincil (kur)** |
| #13 uiux-consultant | mevcut kodu denetle/redesign | **birincil (denetle)** |
| #18 generative-ui | konuşma-içi grafik/diyagram | **birincil (grafik)** |
| #11 frontend-pack | referans CLAUDE.md | `rules/` |
| #19 Ilm-Alan | isim çakışması | yeniden adlandır / ele |
| #20 deveshpunjabi | #12 örtüşür | birini seç |
| #10 design-skills | marka sistemleri | `_staging/` |

### 5.2 SEO / Marketing (5 kaynak)
Teknik SEO çekirdeği = **#14**; geniş marketing = **#32**; growth-ops = **#36**; #7'den yalnız örtüşmeyenler; #27 marketing alt kümesi PM'e bırakılır. İsim/tetikleyici çakışmasını ingest'te çöz.

### 5.3 Agent (genel set)
**SA2 birincil**; .NET = **SA4**; orchestration = SA1-orchestrator'lar; içerik = SA8. SA5 ayrı spec-sistemi (koşullu).

### 5.4 Mega-kataloglar (3+1 kaynak)
#25 (864) · #26 (1.527+) · #33 (880) · SA3 (index). Çoğu otomatik wrapper / yeniden paketlenmiş; #26 diğerlerini kopyalıyor, #25↔#33 ikiz. → `_catalog/` index, **import yok**.

---

## 6. Lisans & güvenlik analizi

| Kategori | Repolar | Aksiyon |
|----------|---------|---------|
| Lisans YOK | #10, 21, 22, 24, 25, 33, SA3, SA5 | `_staging/`/`_catalog/`; ana kütüphaneye almadan upstream netlik bekle |
| Belirsiz lisans | #23 (Remotion ekosistemi) | `_staging/`, niş kullanım |
| CC (atıf gerekli) | #26, #30 | atıf notu; #26 zaten katalog |
| Kısıtlı lisans | #9 | alınmaz |
| Çift kullanım / güvenlik | #31 | **yalnız savunma/blue-team** alt kümesi; saldırı/exploit skill'leri **alınmaz**; ad "Anthropic" içerse de resmî değil |
| IP-hassas | SA7 | agent değil; ana yapıya katma |
| Otomatik çalışma yüzeyi | #14 (hook/network/playwright), #18 (Chrome CDP) | izole kur, hook **opt-in** |
| Skill değil | #28 | `docs/references/` |
| Temiz MIT (sorunsuz) | #11,12,13,15,16,17,19,20,27,32,34,36, SA1,2,4,6,8 | doğrudan ingest |

---

## 7. Aşamalı kurulum (Claude Code komutlarıyla)

> Önkoşul: Node 18+, `claude` CLI, `git`. Tüm repolar `~/.skill-sources/` altına klonlanır, oradan seçilerek kopyalanır.

### Faz 0 — İskelet
```bash
# proje kökünde
mkdir -p .claude/{rules,commands,skills,agents,hooks} _staging _catalog docs/references
mkdir -p ~/.skill-sources
cd your-project && claude   # /init ile başlangıç CLAUDE.md (sonra Bölüm 8.1 ile değiştir)
```

### Faz 1 — Memory
- Kök `CLAUDE.md`'yi **Bölüm 8.1** şablonuyla yaz (<200 satır).

### Faz 2 — Rules
```bash
printf '# Code Style\n' > .claude/rules/code-style.md
printf '# Testing\n'    > .claude/rules/testing.md
printf '# API Conventions\n' > .claude/rules/api-conventions.md
# #11 frontend-pack CLAUDE.md'sini referans olarak:
git clone --depth 1 https://github.com/mikeoptimax/claude-frontend-skills-pack ~/.skill-sources/frontend-pack
cp ~/.skill-sources/frontend-pack/CLAUDE.md .claude/rules/frontend-reference.md
```

### Faz 3 — Skills ingest (öncelik sırası)

**3.1 Temiz yüksek değer (önce bunlar):**
```bash
S=~/.skill-sources
# #15 addyosmani (24, MIT)
git clone --depth 1 https://github.com/addyosmani/agent-skills $S/addy
for d in $S/addy/skills/*/; do n=$(basename "$d"); cp -r "$d" ".claude/skills/dev-$n"; done

# #16 dotnet (100, MIT) — SEÇMELI: ilgili plugin skill'leri
git clone --depth 1 https://github.com/dotnet/skills $S/dotnet
# örn. test + diag + ai plugin'lerinden seç:
find $S/dotnet/plugins -type d -name skills -exec sh -c 'for d in "$1"/*/; do n=$(basename "$d"); cp -r "$d" ".claude/skills/dev-dotnet-$n"; done' _ {} \;

# #17 microsoft (189, MIT) — SEÇMELI: skill-creator + mcp-builder
git clone --depth 1 https://github.com/microsoft/skills $S/msft
cp -r $S/msft/.github/skills/skill-creator .claude/skills/dev-skill-creator
cp -r $S/msft/.github/skills/mcp-builder   .claude/skills/dev-mcp-builder

# #12 premium-ui (tek skill)
git clone --depth 1 https://github.com/ziguishian/premium-ui-builder-skill .claude/skills/fe-premium-ui

# #13 uiux-consultant (tek skill)
git clone --depth 1 https://github.com/Stevenshanmukh/Claude_UI-UX-Design-Consultant-Skill .claude/skills/fe-uiux-audit

# #18 generative-ui (tek skill, Chrome CDP gerektirir)
git clone --depth 1 https://github.com/ai-freer/generative-ui-skill .claude/skills/fe-generative-ui

# #27 pm-skills (68)
git clone --depth 1 https://github.com/phuryn/pm-skills $S/pm
find $S/pm -type d -name skills -exec sh -c 'for d in "$1"/*/; do n=$(basename "$d"); cp -r "$d" ".claude/skills/pm-$n"; done' _ {} \;

# #32 + #36 marketing
git clone --depth 1 https://github.com/coreyhaines31/marketingskills $S/mkt1
for d in $S/mkt1/skills/*/; do n=$(basename "$d"); cp -r "$d" ".claude/skills/mkt-$n"; done
git clone --depth 1 https://github.com/ericosiu/ai-marketing-skills $S/mkt2
for d in $S/mkt2/*/; do n=$(basename "$d"); [ -f "$d/SKILL.md" ] && cp -r "$d" ".claude/skills/mkt-$n"; done

# #34 garden (5)
git clone --depth 1 https://github.com/ConardLi/garden-skills $S/garden
for d in $S/garden/skills/*/; do n=$(basename "$d"); cp -r "$d" ".claude/skills/$n"; done

# #30 academic (4, CC — atıf)
git clone --depth 1 https://github.com/Imbad0202/academic-research-skills $S/acad
for d in $S/acad/*/; do n=$(basename "$d"); [ -f "$d/SKILL.md" ] && cp -r "$d" ".claude/skills/research-$n"; done
```
> Sonra **rm -rf** ile çakışan `.git` klasörlerini temizle: `find .claude/skills -name .git -type d -prune -exec rm -rf {} +`

**3.2 İzole / koşullu:**
```bash
# #14 SEO çekirdek — izole, hook opt-in (install.sh ÇALIŞTIRMA, full-mode motorları kurma)
git clone --depth 1 https://github.com/metawhisp/amazing-seo-skill .claude/skills/seo-technical

# Koşullular → _staging (auto-load edilmez)
git clone --depth 1 https://github.com/master5d/claude-design-skills _staging/master5d
git clone --depth 1 https://github.com/slavingia/skills _staging/slavingia
git clone --depth 1 https://github.com/markdown-viewer/skills _staging/markdown-viewer
git clone --depth 1 https://github.com/ihlamury/design-skills _staging/design-skills
git clone --depth 1 https://github.com/remotion-dev/skills _staging/remotion

# #31 cybersec — SADECE savunma alt kümesi (saldırı/exploit'leri ALMA)
git clone --depth 1 https://github.com/mukul975/Anthropic-Cybersecurity-Skills _staging/cybersec
# _staging'de incele; yalnız detecting-/hunting-/securing-/implementing-/performing-(audit) olanları seç
```

**3.3 Kataloglar (index, import yok):**
```bash
for url in \
  https://github.com/ComposioHQ/awesome-claude-skills \
  https://github.com/ComposioHQ/awesome-codex-skills \
  https://github.com/sickn33/antigravity-awesome-skills ; do
  git clone --depth 1 "$url" "_catalog/$(basename $url)"
done
```

**3.4 Skill değil / referans:**
```bash
git clone --depth 1 https://github.com/florinpop17/app-ideas docs/references/app-ideas
```

### Faz 4 — Hooks
- `.claude/hooks/validate-bash.sh` yaz; `settings.json`'da PreToolUse matcher'ı (Bölüm 8.2). SEO hook'u **manuel/opt-in**.

### Faz 5 — Subagents
```bash
A=~/.skill-sources
# SA2 (37) — genel; MCP/tool referanslarını gözden geçir
git clone --depth 1 https://github.com/lst97/claude-code-sub-agents $A/lst97
cp $A/lst97/agents/*.md .claude/agents/

# SA4 (6) — .NET
git clone --depth 1 https://github.com/Aaronontheweb/dotnet-skills $A/aaron
cp $A/aaron/agents/*.md .claude/agents/

# SA1 — sadece orchestrator'lar
git clone --depth 1 https://github.com/vijaythecoder/awesome-claude-agents $A/vijay
cp $A/vijay/agents/tech-lead-orchestrator.md $A/vijay/agents/team-configurator.md $A/vijay/agents/project-analyst.md .claude/agents/

# SA8 — blog pipeline
git clone --depth 1 https://github.com/AgriciDaniel/claude-blog $A/blog
cp $A/blog/agents/*.md .claude/agents/

# SA5 — spec-workflow → _staging (lisans yok)
git clone --depth 1 https://github.com/zhsama/claude-sub-agent _staging/spec-workflow
```
> SA2 agent'larındaki `mcp__context7__*` / `mcp__sequential-thinking__*` araçlarını ya `.mcp.json`'da kur (Faz 6) ya frontmatter'dan çıkar.

### Faz 6 — MCP
- `.mcp.json`'a takım sunucularını ekle (GitHub, DB, ayrıca SA2'nin beklediği context7/sequential-thinking). Minimum izin.

### Faz 7 — Plugin paketleme
- Olgun kümeleri plugin'e çevir: `dotnet-pack` (#16 + SA4), `frontend-pack` (#12,13,18), `marketing-pack` (#14,32,36).

---

## 8. Bootstrap dosyaları

### 8.1 `CLAUDE.md` (kök — .NET odaklı, görsel 28 referansıyla; düzenle)
```markdown
# Project: <ProjeAdı>

.NET REST API — Clean Architecture + Vertical Slice + CQRS. Claude bu dokümandaki
mimari kurallara uymalı.

## Tech Stack
- Runtime: .NET 10 — ASP.NET Core Minimal APIs
- DB: PostgreSQL — EF Core — Npgsql
- Validation: FluentValidation
- Mimari: Clean Architecture + Vertical Slice + CQRS
- Araçlar: Scalar OpenAPI, Health Checks, Logging Decorators, EF Core Audit Interceptor

## Commands
- dotnet build / dotnet test / dotnet run
- dotnet ef migrations add <Name> / dotnet ef database update

## Architecture Layers
WebApi → Infrastructure → Application → Domain
- Domain: entity + iş kuralları — hiçbir katmana bağımlı değil
- Application: feature (vertical slice) — yalnız Domain'e bağımlı
- Infrastructure: persistence + dış servis — Application+Domain'e bağımlı
- WebApi: giriş noktası — bağımlılıkları ve middleware'i kompoze eder

## Conventions
- Her feature kendi slice'ında (Command/Query + Handler + Validator)
- Skill keşfi: .claude/skills/<ad>/SKILL.md (düz; önek ile gruplanır)
- <200 satır tut; alt-klasör CLAUDE.md'leri üst bağlamı ezmez
```

### 8.2 `.claude/settings.json`
```json
{
  "permissions": {
    "allow": ["Read:**", "Bash:git *", "Write:**/*.md"],
    "deny": ["Read:env:*", "Bash:sudo *", "Read:.env*"]
  },
  "hooks": {
    "PreToolUse": [
      { "matcher": "Bash",
        "hooks": [{ "type": "command", "command": ".claude/hooks/validate-bash.sh", "timeout": 5 }] }
    ]
  }
}
```
> Exit code: 0 → allow, 2 → block.

### 8.3 `.gitignore` (eklenecek satırlar)
```
CLAUDE.local.md
.claude/settings.local.json
_staging/
_catalog/
```
> `_staging/`, `_catalog/` repo'ya girmesin (lisanssız/devasa içerik).

---

## 9. Doğrulama & bakım
- `claude` aç → `/doctor` (tam tanı), `/context` (yüklenen skill/agent/hook), `/permissions`.
- Skill görünmüyorsa: yol `.claude/skills/<ad>/SKILL.md` mı? `description` bir TRIGGER mı?
- `.git` artıkları temizlendi mi? (`find .claude -name .git -prune -exec rm -rf {} +`)
- Context %70'i geçince `/compact`, %90+ `/clear`.
- Yeni repo eklerken: lisans → boyut → çakışma → hedef önek sırasıyla değerlendir.

---

## 10. Hızlı öncelik özeti

**Hemen (temiz, yüksek değer):**
Skills → #15, #16(seç), #17(skill-creator+mcp-builder), #12, #13, #18, #27, #32, #36, #34, #30 · #14(izole)
Agents → SA2, SA4, SA1-orchestrator, SA8

**Koşullu → `_staging/`:** #10, 21, 22, 23, 24, 30(atıf), 31(savunma) · SA5

**Katalog → `_catalog/` (import yok):** #25, 26, 33 · SA3

**Alma:** #9, #28(skill değil), SA7(agent değil/IP), #19/#20(frontend çakışma), #31-saldırı, #29(bilim yapmıyorsan)

**Çözülecek çakışmalar:** frontend 8→4 rol · SEO/marketing 5→3 · agent genel set SA1↔SA2 · 3 mega-katalog
