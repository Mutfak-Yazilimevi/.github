#!/usr/bin/env bash
# SessionEnd hook — oturum sonunda kısa bir kapanış işareti bırakır.
# agent-memory/ altına zaman damgalı bir not ekler ki kalıcı hafıza damıtması hatırlansın.
# Asla akışı bozma (0). Hassas veri YAZMA.

set -uo pipefail

mem_dir=".claude/agent-memory"
[ -d "$mem_dir" ] || exit 0   # dizin yoksa sessiz çık

log="$mem_dir/_session-log.md"
ts="$(date -u +%Y-%m-%dT%H:%M:%SZ 2>/dev/null || echo unknown)"

# Yalnız işaret bırak; özet üretmek Claude'a aittir (bu script model değildir).
{
  printf -- '- %s — oturum sonlandı. Kalıcı kararları ilgili `agent-memory/<agent>.md`'"'"'ye damıtmayı unutma.\n' "$ts"
} >> "$log" 2>/dev/null || true

exit 0
