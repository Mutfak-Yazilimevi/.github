#!/usr/bin/env bash
# SessionStart hook — oturum başında kısa bir bağlam notu yazar.
# stdout, oturum bağlamına eklenir; KISA tut (token). Asla akışı bozma (0).

set -uo pipefail

cat <<'NOTE'
[proje bağlamı] .NET REST API — Clean Architecture + Vertical Slice + CQRS.
Kurallar: .claude/rules/ (code-style, testing, api-conventions, architecture, scaling).
Skill/agent merkezi marketplace'ten gelir; tasarım altitüdü: dev-low-level-design /
dev-dotnet-architecture-selection / dev-system-design-scaling. Yetenek boşluğunda
.claude/rules/capability-gaps.md akışını uygula.
NOTE

exit 0
