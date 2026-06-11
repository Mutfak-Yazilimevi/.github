# `.claude/` — Claude Code Yapılandırması

Bu dizin, Claude Code'un proje kapsamındaki (project scope) yapılandırmasını tutar.
Master plan: [`../CLAUDE-CODE-MASTERPLAN.md`](../CLAUDE-CODE-MASTERPLAN.md) · İlerleme: [`../TASK_LIST.md`](../TASK_LIST.md)

## Dizin yapısı (Faz 0 — İskelet)

| Klasör | Amaç |
| :--- | :--- |
| `rules/` | Kod stili, test, API kuralları (`code-style.md`, `testing.md`, `api-conventions.md`) — Faz 2 |
| `commands/` | Slash komutları (`review.md`, `fix-issue.md`, `deploy.md`) |
| `skills/` | ⚠️ **DÜZ yapı** — her skill `skills/<önek-ad>/SKILL.md` olarak yerleşir; iç içe kategori klasörü otomatik bulunmaz |
| `agents/` | Sub-agent tanımları (`<ad>.md`, düz) — Faz 5 |
| `hooks/` | Guardrail script'leri (`validate-bash.sh`) — Faz 4 |

## Skill keşif kuralı (kritik)

Claude Code skill'leri **yalnızca** `.claude/skills/<ad>/SKILL.md` yolundan tarar.
Gruplama için **ad öneki** kullanılır: `dev-`, `fe-`, `seo-`, `mkt-`, `pm-`, `design-`, `research-`, `sec-`.

`_staging/`, `_catalog/` ve `docs/` **asla** `.claude/skills/` altında olmaz (auto-load edilmez).
