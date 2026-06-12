# Project: <ProjeAdı>

.NET REST API — Clean Architecture + Vertical Slice + CQRS. Claude bu dokümandaki
mimari kurallara uymalı. Bu dosya takım talimatıdır (commit edilir); kişisel notlar
`CLAUDE.local.md` içine yazılır (gitignore).

> Şablon: `<...>` alanlarını projeye göre doldur. Dosyayı <200 satır tut.

## Tech Stack

- **Runtime:** .NET 10 — ASP.NET Core Minimal APIs
- **DB:** PostgreSQL — EF Core — Npgsql
- **Validation:** FluentValidation
- **Mimari:** Clean Architecture + Vertical Slice + CQRS
- **Araçlar:** Scalar OpenAPI, Health Checks, Logging Decorators, EF Core Audit Interceptor

## Commands

- `dotnet build` — derle
- `dotnet test` — testleri çalıştır
- `dotnet run` — uygulamayı çalıştır
- `dotnet ef migrations add <Name>` — migration ekle
- `dotnet ef database update` — veritabanını güncelle

## Architecture Layers

```
WebApi → Infrastructure → Application → Domain
```

- **Domain:** entity + iş kuralları — hiçbir katmana bağımlı değil
- **Application:** feature (vertical slice) — yalnız Domain'e bağımlı
- **Infrastructure:** persistence + dış servis — Application + Domain'e bağımlı
- **WebApi:** giriş noktası — bağımlılıkları ve middleware'i kompoze eder

## Conventions

- Her feature kendi slice'ında: Command/Query + Handler + Validator
- Skill keşfi: `.claude/skills/<ad>/SKILL.md` (düz; önek ile gruplanır — `dev-`, `fe-`, `mkt-`, `pm-`, ...)
- Bu dosyayı <200 satır tut; alt-klasör `CLAUDE.md`'leri üst bağlamı **ezmez**, tamamlar
- **Yetenek boşluğu:** Karşılığı olmayan (mevcut skill/agent kapsamayan) bir konu çıkarsa,
  tek seferlik çözmek yerine merkezî kütüphaneye yeni skill/agent üretmeyi **öner** → bkz.
  `.claude/rules/capability-gaps.md`
- Kurallar `.claude/rules/` altında: `code-style.md`, `testing.md`, `api-conventions.md`,
  `architecture.md`, `scaling.md`, `mcp.md`, `process.md`, `catalog.md`, `capability-gaps.md`
- **Katalog-öncelikli (her soru/plan/iş):** önce `skills/skills-catalog.csv` ve
  `agents/agents-catalog.csv`'yi tara → uygun skill/agent'ı bul → ona git. Yoksa
  `capability-gaps.md`. Yeni ekleyince katalogu yeniden üret (`build-catalog.py`). Bkz. `rules/catalog.md`.
- **Süreç:** işi tanımlı yaşam döngüsüyle yürüt (SDLC/ADLC) — `rules/process.md` +
  `dev-development-lifecycle` skill'i; önemli feature'larda `mutfak-spec-workflow` zinciri
- Slash komutları `.claude/commands/`: `/intake` (yeni proje ön kapısı), `/onboard` (mevcut proje,
  salt-okunur analiz→backlog), `/review`, `/fix-issue`, `/deploy`, `/test-all`, `/bootstrap`,
  `/document`, `/refactor`
- **Başlangıç:** yeni proje → `/intake` (rehberli brief → PRD) · mevcut proje → `/onboard`
  (kod içinde anla, yapılan/eksik çıkar, `docs/backlog.md`'ye yaz, direktif bekle)
- Hook'lar `.claude/hooks/` (taksonomi: `hooks/README.md`) — Pre/PostToolUse, SessionStart/End,
  PreCompact (sır taraması), Notification/Stop. MCP sunucuları: `.mcp.json` (bkz. `rules/mcp.md`)
- **Tasarım altitüdü:** sınıf/nesne seviyesi → `dev-low-level-design` · mimari seçim →
  `dev-dotnet-architecture-selection` (+ `dotnet-backend-architect` agent) · ölçekleme →
  `dev-system-design-scaling`

## Memory Hiyerarşisi

global (`~/.claude/`) → monorepo kök → proje (`./CLAUDE.md`) → alt-klasör.
Her memory dosyası <200 satır; üst bağlamı geçersiz kılmaz.

## Context Yönetimi

- %50–70: izle
- %70–90: `/compact`
- %90+: `/clear`
