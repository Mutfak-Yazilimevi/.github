#!/usr/bin/env bash
# PreCompact hook — "secret detection gate".
# Context sıkıştırmadan önce, son değişikliklerde sır benzeri desenleri tarar ve UYARIR.
# Engellemez (exit 0); amaç sırların özete/log'a sızmadan önce fark edilmesi.

set -uo pipefail

command -v git >/dev/null 2>&1 || exit 0
git rev-parse --is-inside-work-tree >/dev/null 2>&1 || exit 0

# Taranacak içerik: çalışma ağacındaki değişiklikler (staged + unstaged)
diff="$(git diff HEAD 2>/dev/null || true)"
[ -z "$diff" ] && exit 0

patterns=(
  'AKIA[0-9A-Z]{16}'                         # AWS access key
  'ghp_[0-9A-Za-z]{36}'                      # GitHub PAT
  'xox[baprs]-[0-9A-Za-z-]+'                 # Slack token
  '-----BEGIN[[:space:]].*PRIVATE KEY-----'  # private key
  '(password|passwd|secret|api[_-]?key|token)[[:space:]]*[:=][[:space:]]*['\''"][^'\''"]{8,}'
)

found=0
for p in "${patterns[@]}"; do
  if printf '%s' "$diff" | grep -Eiq -e "$p"; then
    [ "$found" = 0 ] && echo "⚠ Olası sır tespit edildi (PreCompact). Compaction öncesi kontrol edin:" >&2
    echo "  - desen: $p" >&2
    found=1
  fi
done

# Uyarı; engelleme yok.
exit 0
