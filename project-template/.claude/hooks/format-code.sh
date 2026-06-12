#!/usr/bin/env bash
# PostToolUse hook — Write/Edit sonrası düzenlenen dosyayı biçimlendirir (best-effort).
# Asla başarısız olup akışı bozmaz: araç yoksa veya dosya uygun değilse sessizce çıkar (0).
# Girdi: stdin'den hook JSON'u (tool_input.file_path).

set -uo pipefail

input="$(cat)"
if command -v jq >/dev/null 2>&1; then
  file="$(printf '%s' "$input" | jq -r '.tool_input.file_path // .tool_input.path // empty')"
else
  exit 0   # jq yoksa güvenli tarafta kal
fi
[ -z "${file:-}" ] && exit 0
[ -f "$file" ] || exit 0

case "$file" in
  *.cs)
    # .NET: yalnız dotnet kuruluysa ve bir proje/solution varsa
    if command -v dotnet >/dev/null 2>&1; then
      dotnet format --include "$file" >/dev/null 2>&1 || true
    fi
    ;;
  *.ts|*.tsx|*.js|*.jsx|*.json|*.css|*.md)
    # Frontend: yalnız prettier erişilebilirse
    if command -v npx >/dev/null 2>&1; then
      npx --no-install prettier --write "$file" >/dev/null 2>&1 || true
    fi
    ;;
esac

exit 0
