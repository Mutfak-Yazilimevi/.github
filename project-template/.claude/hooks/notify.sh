#!/usr/bin/env bash
# Notification / Stop hook — opsiyonel Slack/webhook bildirimi.
# SADECE SLACK_WEBHOOK_URL ortam değişkeni set ise gönderir; değilse sessizce çıkar (0).
# Akışı asla bozmaz. Girdi: stdin'den hook JSON'u.

set -uo pipefail

input="$(cat)"
[ -z "${SLACK_WEBHOOK_URL:-}" ] && exit 0
command -v curl >/dev/null 2>&1 || exit 0

# Mesajı çıkar (jq varsa)
msg="Claude Code: oturum olayı"
if command -v jq >/dev/null 2>&1; then
  m="$(printf '%s' "$input" | jq -r '.message // .reason // empty' 2>/dev/null)"
  [ -n "$m" ] && msg="Claude Code: $m"
fi

curl -fsS -X POST -H 'Content-Type: application/json' \
  --data "$(printf '{"text":%s}' "$(printf '%s' "$msg" | jq -R . 2>/dev/null || printf '"%s"' "$msg")")" \
  "$SLACK_WEBHOOK_URL" >/dev/null 2>&1 || true

exit 0
