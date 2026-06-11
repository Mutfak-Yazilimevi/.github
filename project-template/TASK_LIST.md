# 📋 Görev Listesi — Claude Code Skill & Sub-Agent Kütüphanesi

> Kaynak: [`CLAUDE-CODE-MASTERPLAN.md`](./CLAUDE-CODE-MASTERPLAN.md)
> Bu liste master planı uygulanabilir görevlere böler. Biten görevler `[x]` ile işaretlenir.
>
> **Durum anahtarı:** `[x]` tamamlandı · `[ ]` bekliyor · `[~]` işlendi/devam ediyor
> **Son güncelleme:** 2026-06-11

---

## Faz 0 — İskelet

- [x] Klasör yapısını oluştur: `.claude/{rules,commands,skills,agents,hooks}`, `_staging`, `_catalog`, `docs/references` ✅
- [x] `~/.skill-sources` klonlama dizinini oluştur ✅ _(kaynak repolar buraya klonlandı, seçilenler kopyalandı)_
- [ ] `claude` ile başlangıç `CLAUDE.md` üret (`/init`) _(Faz 1'de Bölüm 8.1 şablonuyla yazılacak)_

## Faz 1 — Memory

- [x] Kök `CLAUDE.md` dosyasını Bölüm 8.1 şablonuyla yaz (<200 satır, .NET odaklı) ✅

## Faz 2 — Rules

- [x] `.claude/rules/code-style.md` oluştur ✅
- [x] `.claude/rules/testing.md` oluştur ✅
- [x] `.claude/rules/api-conventions.md` oluştur ✅
- [~] #11 frontend-pack `CLAUDE.md`'sini `rules/frontend-reference.md` olarak ekle _(referans/placeholder oluşturuldu; upstream içerik kurulumda çekilecek)_

## Faz 3 — Skills Ingest

### 3.1 Temiz / yüksek değer

- [x] #1 — [anthropics/skills](https://github.com/anthropics/skills) (17) → `skills/` **eklendi**
- [x] #2 — [keenthemes/reui](https://github.com/keenthemes/reui) (6) → `skills/ design-` **eklendi**
- [~] #3 — [mattpocock/skills](https://github.com/mattpocock/skills) → `skills/ dev-` (işlendi)
- [~] #4 — [openai/skills](https://github.com/openai/skills) → `skills/ dev-` (işlendi)
- [~] #5 — [MiniMax-AI/skills](https://github.com/MiniMax-AI/skills) → `skills/ dev-` (işlendi)
- [~] #6 — [obra/superpowers](https://github.com/obra/superpowers) → `skills/ dev-` (işlendi)
- [~] #7 — [alirezarezvani/claude-skills](https://github.com/alirezarezvani/claude-skills) → `skills/ mkt-` (dedupe, işlendi)
- [~] #8 — [cloudflare/skills](https://github.com/cloudflare/skills) (9) → `skills/ dev-` (işlendi)
- [x] #15 — [addyosmani/agent-skills](https://github.com/addyosmani/agent-skills) (24, MIT) → `dev-` ✅
- [x] #16 — [dotnet/skills](https://github.com/dotnet/skills) (100, MIT) → `dev-dotnet-` ✅ **seçmeli: `dotnet-test` + `dotnet-diag` + `dotnet-ai` plugin'leri**
- [x] #17 — [microsoft/skills](https://github.com/microsoft/skills) (189, MIT) → `dev-skill-creator` + `dev-mcp-builder` ✅
- [x] #12 — [ziguishian/premium-ui-builder-skill](https://github.com/ziguishian/premium-ui-builder-skill) (1) → `fe-premium-ui` ✅
- [x] #13 — [Stevenshanmukh/UI-UX-Consultant](https://github.com/Stevenshanmukh/Claude_UI-UX-Design-Consultant-Skill) (1) → `fe-uiux-audit` ✅
- [x] #18 — [ai-freer/generative-ui-skill](https://github.com/ai-freer/generative-ui-skill) (1, Chrome CDP) → `fe-generative-ui` ✅
- [x] #27 — [phuryn/pm-skills](https://github.com/phuryn/pm-skills) (68, MIT) → `pm-` ✅
- [x] #32 — [coreyhaines31/marketingskills](https://github.com/coreyhaines31/marketingskills) (44, MIT) → `mkt-` ✅
- [x] #36 — [ericosiu/ai-marketing-skills](https://github.com/ericosiu/ai-marketing-skills) (22, MIT) → `mkt-` ✅
- [x] #34 — [ConardLi/garden-skills](https://github.com/ConardLi/garden-skills) (5, MIT) → `skills/` ✅
- [x] #30 — [Imbad0202/academic-research-skills](https://github.com/Imbad0202/academic-research-skills) (4, CC — atıf) → `research-` ✅
- [x] `.git` artıklarını temizle ✅ (kopyalama sırasında strip edildi — 0 artık)

### 3.2 İzole / koşullu → `_staging/`

- [ ] #14 — [metawhisp/amazing-seo-skill](https://github.com/metawhisp/amazing-seo-skill) → `skills/ seo-technical` (izole, hook opt-in, install.sh ÇALIŞTIRMA)
- [ ] #21 — [master5d/claude-design-skills](https://github.com/master5d/claude-design-skills) → `_staging/` (lisans yok)
- [ ] #22 — [slavingia/skills](https://github.com/slavingia/skills) → `_staging/ pm-` (lisans yok)
- [ ] #24 — [markdown-viewer/skills](https://github.com/markdown-viewer/skills) → `_staging/` (lisans yok)
- [ ] #10 — [ihlamury/design-skills](https://github.com/ihlamury/design-skills) (87) → `_staging/` (lisans yok)
- [ ] #23 — [remotion-dev/skills](https://github.com/remotion-dev/skills) → `_staging/` (niş, lisans belirsiz)
- [ ] #31 — [Anthropic-Cybersecurity-Skills](https://github.com/mukul975/Anthropic-Cybersecurity-Skills) → `_staging/` (YALNIZ savunma alt kümesi; saldırı/exploit ALMA)

### 3.3 Kataloglar (index, import yok) → `_catalog/`

- [ ] #25 — [ComposioHQ/awesome-claude-skills](https://github.com/ComposioHQ/awesome-claude-skills) (864) → `_catalog/`
- [ ] #33 — [ComposioHQ/awesome-codex-skills](https://github.com/ComposioHQ/awesome-codex-skills) (880) → `_catalog/`
- [ ] #26 — [sickn33/antigravity-awesome-skills](https://github.com/sickn33/antigravity-awesome-skills) (1.527+) → `_catalog/`

### 3.4 Skill değil / referans

- [ ] #28 — [florinpop17/app-ideas](https://github.com/florinpop17/app-ideas) → `docs/references/app-ideas`

### Alınmayacaklar (karar verildi)

- [x] #9 — TRAE-Skills: kısıtlı lisans → **alınmadı**
- [x] #19 / #20 — frontend isim çakışması → #12 ile çözülecek (ele/yeniden adlandır)
- [x] #29 — scientific-agent-skills (144): niş, bilim yapılmıyorsa → **alınmıyor**

## Faz 4 — Hooks

- [x] `.claude/hooks/validate-bash.sh` yaz ✅ (tehlikeli desen → exit 2; test edildi)
- [x] `settings.json`'a `PreToolUse` Bash matcher ekle (Bölüm 8.2) ✅
- [~] SEO hook'unu manuel/opt-in olarak işaretle _(SEO skill #14 henüz ingest edilmedi — Faz 3.2 `_staging`; eklenince geçerli)_

## Faz 5 — Sub-Agents

- [x] SA2 — [lst97/claude-code-sub-agents](https://github.com/lst97/claude-code-sub-agents) (37, MIT) → `agents/` ✅ **birincil** ⚠️ MCP/tool referansları Faz 6'da gözden geçirilecek
- [x] SA4 — [Aaronontheweb/dotnet-skills](https://github.com/Aaronontheweb/dotnet-skills) (6, MIT) → `agents/` ✅ **.NET gold**
- [x] SA1 — [vijaythecoder/awesome-claude-agents](https://github.com/vijaythecoder/awesome-claude-agents): `tech-lead-orchestrator`, `team-configurator`, `project-analyst` ✅
- [x] SA8 — [AgriciDaniel/claude-blog](https://github.com/AgriciDaniel/claude-blog) (5, MIT) → blog pipeline ✅
- [ ] SA5 — [zhsama/claude-sub-agent](https://github.com/zhsama/claude-sub-agent) (12) → `_staging/` (lisans yok)
- [x] SA3 — a-list-of (index) → `_catalog/`, kaynak değil → **karar: katalog**
- [x] SA6 — claude-seo (1): minimal → **karar verildi**
- [x] SA7 — claude-code-system-prompts (352): agent değil / IP-hassas → **alınmıyor**

## Faz 6 — MCP

- [~] `.mcp.json`'a takım sunucularını ekle (GitHub, DB) _(GitHub/DB proje-özel, token/connection string gerektirir — kurulumda eklenir)_
- [x] SA2'nin beklediği `context7` ve `sequential-thinking` MCP'lerini kur ✅ (`.mcp.json`'a eklendi)

## Faz 7 — Plugin Paketleme

- [ ] `dotnet-pack` plugin'i (#16 + SA4)
- [ ] `frontend-pack` plugin'i (#12, #13, #18)
- [ ] `marketing-pack` plugin'i (#14, #32, #36)

## Bootstrap Dosyaları (Bölüm 8)

- [x] `CLAUDE.md` (kök, .NET odaklı şablon) ✅ (Faz 1)
- [x] `.claude/settings.json` (permissions + hooks) ✅
- [x] `.gitignore`'a ekle: `CLAUDE.local.md`, `.claude/settings.local.json`, `_staging/`, `_catalog/` ✅

## Doğrulama & Bakım (Bölüm 9)

- [ ] `/doctor` ile tam tanı
- [ ] `/context` ile yüklenen skill/agent/hook doğrula
- [ ] `/permissions` kontrol
- [ ] Skill yolu `.claude/skills/<ad>/SKILL.md` doğrulandı mı?
- [ ] `.git` artıkları temizlendi mi?

---

### 📊 İlerleme Özeti

| Faz | Toplam | Biten | Durum |
|-----|--------|-------|-------|
| Faz 0 — İskelet | 3 | 2 | 🔄 |
| Faz 1 — Memory | 1 | 1 | ✅ |
| Faz 2 — Rules | 4 | 3 | 🔄 |
| Faz 3 — Skills | 33 | 17 | 🔄 (209 skill ingest edildi) |
| Faz 4 — Hooks | 3 | 2 | 🔄 |
| Faz 5 — Agents | 8 | 7 | 🔄 (51 agent ingest edildi) |
| Faz 6 — MCP | 2 | 1 | 🔄 |
| Faz 7 — Plugin | 3 | 0 | ⏳ |
| Bootstrap | 3 | 3 | ✅ |
| Doğrulama | 5 | 0 | ⏳ |

> Not: "Biten" sütunu, master planda `✅ eklendi` veya kesin karar (`❌ alınmadı`) olarak işaretlenmiş kalemleri kapsar. `⏳ işlendi` durumundakiler `[~]` ile gösterilir ve henüz tamamlanmış sayılmaz.
