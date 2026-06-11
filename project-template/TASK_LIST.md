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
- [x] `claude` ile başlangıç `CLAUDE.md` üret (`/init`) ✅ _(Faz 1'de Bölüm 8.1 şablonuyla yazıldı)_

## Faz 1 — Memory

- [x] Kök `CLAUDE.md` dosyasını Bölüm 8.1 şablonuyla yaz (<200 satır, .NET odaklı) ✅

## Faz 2 — Rules

- [x] `.claude/rules/code-style.md` oluştur ✅
- [x] `.claude/rules/testing.md` oluştur ✅
- [x] `.claude/rules/api-conventions.md` oluştur ✅
- [x] #11 frontend-pack `CLAUDE.md`'sini `rules/frontend-reference.md` olarak ekle ✅ (upstream MIT içerik gömüldü — 21st.dev/Next.js 15 frontend kuralları)

## Faz 3 — Skills Ingest

### 3.1 Temiz / yüksek değer

- [x] #1 — [anthropics/skills](https://github.com/anthropics/skills) → `skills/` ✅ (**force-all: tümü**; 10 Apache-2.0 + `docx/pdf/pptx/xlsx` ⚠️ source-available + `doc-coauthoring`; `skill-creator`/`mcp-builder` zaten `dev-`)
- [x] #2 — [keenthemes/reui](https://github.com/keenthemes/reui) → `skills/ design-` ✅ (**6 skill**, MIT; `reui-seo` dup tekleştirildi)
- [x] #3 — [mattpocock/skills](https://github.com/mattpocock/skills) (29, MIT) → `dev-` ✅
- [x] #4 — [openai/skills](https://github.com/openai/skills) (44) → `dev-` ✅ (⚠️ lisans yok — force-all)
- [x] #5 — [MiniMax-AI/skills](https://github.com/MiniMax-AI/skills) (23, MIT) → `dev-` ✅
- [x] #6 — [obra/superpowers](https://github.com/obra/superpowers) (14, MIT) → `dev-` ✅
- [x] #7 — [alirezarezvani/claude-skills](https://github.com/alirezarezvani/claude-skills) → `ali-` ✅ (**hepsi: 347 görünür skill**; `.gemini` vb. 410 tool-mirror kopya hariç; çakışmalar kategori ekiyle korundu)
- [x] #8 — [cloudflare/skills](https://github.com/cloudflare/skills) (9, MIT) → `dev-` ✅
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

- [x] #14 — [metawhisp/amazing-seo-skill](https://github.com/metawhisp/amazing-seo-skill) → `skills/seo-technical` ✅ (Apache-2.0, izole; install.sh **çalıştırılmadı**)
- [x] #21 — [master5d/claude-design-skills](https://github.com/master5d/claude-design-skills) (6) → `design-` ✅ (force-all; ⚠️ lisans yok)
- [x] #22 — [slavingia/skills](https://github.com/slavingia/skills) (10) → `pm-` ✅ (force-all; ⚠️ lisans yok)
- [x] #24 — [markdown-viewer/skills](https://github.com/markdown-viewer/skills) (15) → `md-` ✅ (force-all; ⚠️ lisans yok)
- [x] #10 — [ihlamury/design-skills](https://github.com/ihlamury/design-skills) (87) → `design-` ✅ (force-all; ⚠️ lisans yok)
- [x] #23 — [remotion-dev/skills](https://github.com/remotion-dev/skills) (1) → `remotion` ✅ (force-all; ⚠️ lisans belirsiz)
- [x] #31 — [Anthropic-Cybersecurity-Skills](https://github.com/mukul975/Anthropic-Cybersecurity-Skills) → `sec-` ✅ (force-all: **tam 754 skill, filtresiz** — ⚠️ saldırgan/exploit dahil)

### 3.3 Kataloglar (index, import yok) → `_catalog/`

- [x] #25 — [ComposioHQ/awesome-claude-skills](https://github.com/ComposioHQ/awesome-claude-skills) (864) → `_catalog/` ✅ (force-add; ⚠️ lisans yok)
- [x] #33 — [ComposioHQ/awesome-codex-skills](https://github.com/ComposioHQ/awesome-codex-skills) (880) → `_catalog/` ✅ (force-add; ⚠️ lisans yok)
- [x] #26 — [sickn33/antigravity-awesome-skills](https://github.com/sickn33/antigravity-awesome-skills) (4850) → `_catalog/` ✅ (force-add; CC BY 4.0+MIT; ⚠️ ~249MB)

### 3.4 Skill değil / referans

- [x] #28 — [florinpop17/app-ideas](https://github.com/florinpop17/app-ideas) → `docs/references/app-ideas` ✅ (MIT)

### Alınmayacaklar (karar verildi)

- [x] #9 — TRAE-Skills: kısıtlı lisans → **alınmadı**
- [x] #19 / #20 — frontend isim çakışması → #12 ile çözülecek (ele/yeniden adlandır)
- [x] #29 — scientific-agent-skills (144): niş, bilim yapılmıyorsa → **alınmıyor**

## Faz 4 — Hooks

- [x] `.claude/hooks/validate-bash.sh` yaz ✅ (tehlikeli desen → exit 2; test edildi)
- [x] `settings.json`'a `PreToolUse` Bash matcher ekle (Bölüm 8.2) ✅
- [x] SEO hook'unu manuel/opt-in olarak işaretle ✅ (#14 `seo-technical` ingest edildi; hook `settings.json`'a **bağlanmadı** = opt-in; yalnız `validate-bash` aktif)

## Faz 5 — Sub-Agents

- [x] SA2 — [lst97/claude-code-sub-agents](https://github.com/lst97/claude-code-sub-agents) (37, MIT) → `agents/` ✅ **birincil** ⚠️ MCP/tool referansları Faz 6'da gözden geçirilecek
- [x] SA4 — [Aaronontheweb/dotnet-skills](https://github.com/Aaronontheweb/dotnet-skills) (6, MIT) → `agents/` ✅ **.NET gold**
- [x] SA1 — [vijaythecoder/awesome-claude-agents](https://github.com/vijaythecoder/awesome-claude-agents): `tech-lead-orchestrator`, `team-configurator`, `project-analyst` ✅
- [x] SA8 — [AgriciDaniel/claude-blog](https://github.com/AgriciDaniel/claude-blog) (5, MIT) → blog pipeline ✅
- [x] SA5 — [zhsama/claude-sub-agent](https://github.com/zhsama/claude-sub-agent) (12) → `agents/` ✅ (force-all; ⚠️ lisans yok)
- [x] SA3 — a-list-of (index) → `_catalog/`, kaynak değil → **karar: katalog**
- [x] SA6 — claude-seo (1): minimal → **karar verildi**
- [x] SA7 — claude-code-system-prompts (352): agent değil / IP-hassas → **alınmıyor**

## Faz 6 — MCP

- [~] `.mcp.json`'a takım sunucularını ekle (GitHub, DB) _(GitHub/DB proje-özel, token/connection string gerektirir — kurulumda eklenir)_
- [x] SA2'nin beklediği `context7` ve `sequential-thinking` MCP'lerini kur ✅ (`.mcp.json`'a eklendi)

## Faz 7 — Plugin Paketleme

- [x] `dotnet-pack` plugin'i (#16 + SA4) ✅ — manifest + `build-plugins.sh` (37 skill + 6 agent)
- [x] `frontend-pack` plugin'i (#12, #13, #18) ✅ — manifest (3 skill)
- [x] `marketing-pack` plugin'i (#14, #32, #36) ✅ — manifest (67 skill)

## Bootstrap Dosyaları (Bölüm 8)

- [x] `CLAUDE.md` (kök, .NET odaklı şablon) ✅ (Faz 1)
- [x] `.claude/settings.json` (permissions + hooks) ✅
- [x] `.gitignore`'a ekle: `CLAUDE.local.md`, `.claude/settings.local.json`, `_staging/`, `_catalog/` ✅

## Doğrulama & Bakım (Bölüm 9)

- [~] `/doctor` ile tam tanı _(interaktif — proje açılışında çalıştırılır)_
- [x] `/context` ile yüklenen skill/agent/hook doğrula ✅ _(1606 skill + 63 agent Claude Code tarafından keşfedildi)_
- [~] `/permissions` kontrol _(interaktif; `settings.json` allow/deny tanımlı)_
- [x] Skill yolu `.claude/skills/<ad>/SKILL.md` doğrulandı mı? ✅ (düz yapı, önekli)
- [x] `.git` artıkları temizlendi mi? ✅ (0 artık)

---

### 📊 İlerleme Özeti

| Faz | Toplam | Biten | Durum |
|-----|--------|-------|-------|
| Faz 0 — İskelet | 3 | 3 | ✅ |
| Faz 1 — Memory | 1 | 1 | ✅ |
| Faz 2 — Rules | 4 | 4 | ✅ |
| Faz 3 — Skills | 33 | 34 | ✅ (1606 aktif skill + kataloglar force-add) |
| Faz 4 — Hooks | 3 | 3 | ✅ |
| Faz 5 — Agents | 8 | 8 | ✅ (63 agent) |
| Faz 6 — MCP | 2 | 1 | 🔄 (GitHub/DB MCP = proje-özel token) |
| Faz 7 — Plugin | 3 | 3 | ✅ |
| Bootstrap | 3 | 3 | ✅ |
| Doğrulama | 5 | 3 | 🔄 (`/doctor`,`/permissions` = interaktif) |

> **Tüm tamamlanabilir görevler bitti.** Geriye yalnızca **template'te yapısal olarak
> tamamlanamayan** kalemler kaldı:
> - **Mega-kataloglar (#25/#33/#26):** plan gereği index-only, "import yok" (`setup.sh --catalog` ile yerel).
> - **GitHub/DB MCP:** proje-özel token/connection gerektirir.
> - **`/doctor`, `/permissions`:** interaktif Claude komutları (proje açılışında çalışır).

> Not: "Biten" sütunu, master planda `✅ eklendi` veya kesin karar (`❌ alınmadı`) olarak işaretlenmiş kalemleri kapsar. `⏳ işlendi` durumundakiler `[~]` ile gösterilir ve henüz tamamlanmış sayılmaz.
