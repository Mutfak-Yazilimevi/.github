#!/usr/bin/env bash
# PreToolUse guardrail — Bash komutlarını doğrular.
# Çıkış kodu: 0 → izin ver · 2 → engelle (stderr Claude'a geri verilir).
# Girdi: stdin'den hook JSON'u (tool_input.command alanı).

set -euo pipefail

# Hook girdisini oku
input="$(cat)"

# Komutu çıkar (jq varsa onunla, yoksa kaba grep ile)
if command -v jq >/dev/null 2>&1; then
  cmd="$(printf '%s' "$input" | jq -r '.tool_input.command // empty')"
else
  cmd="$input"
fi

[ -z "$cmd" ] && exit 0

# --- Engellenen tehlikeli desenler ---
deny_patterns=(
  'rm[[:space:]]+-rf[[:space:]]+/'        # rm -rf / kök silme
  ':\(\)\{.*\|.*&\}'                       # fork bomb
  'mkfs'                                   # disk formatlama
  'dd[[:space:]]+if=.*of=/dev/'           # disk üzerine yazma
  '>[[:space:]]*/dev/sd'                   # ham diske yazma
  'curl[[:space:]].*\|[[:space:]]*sh'      # curl | sh (denetimsiz uzak script)
  'wget[[:space:]].*\|[[:space:]]*sh'      # wget | sh
  'chmod[[:space:]]+-R[[:space:]]+777'     # aşırı geniş izin
  'sudo[[:space:]]'                        # yükseltilmiş yetki
)

for p in "${deny_patterns[@]}"; do
  if printf '%s' "$cmd" | grep -Eq "$p"; then
    echo "⛔ Engellendi: '$cmd' tehlikeli desen içeriyor ($p)." >&2
    echo "Gerekliyse komutu daraltıp tekrar deneyin veya manuel onay alın." >&2
    exit 2
  fi
done

exit 0
