#!/usr/bin/env bash
# publish.sh — Mutfak plugin marketplace'ini claude-config reposuna yayımlar.
#
# NEREDE ÇALIŞTIRILIR: claude-config reposunun KÖKÜNDE (cwd = repo kökü),
# yani claude-config'i kapsamına alan bir Claude Code oturumunda.
#
# NE YAPAR:
#   1) Public .github reposunu klonlar (scaffold + skill/agent kütüphanesi kaynağı)
#   2) claude-config scaffold'unu (marketplace.json, manifestler, scripts, templates) buraya kopyalar
#   3) build-marketplace.sh ile 12 plugin'i doldurur (skill/agent materyalize)
#   4) commit + push -> marketplace canlı
#
# Tekrar çalıştırılabilir (idempotent): kütüphane güncellenince yeniden yayımlar.
set -euo pipefail

GH_REPO="${GH_REPO:-https://github.com/Mutfak-Yazilimevi/.github}"
TMP="$(mktemp -d)"; trap 'rm -rf "$TMP"' EXIT

echo "→ Kaynak (.github) klonlanıyor…"
git clone --depth 1 "$GH_REPO" "$TMP/gh" >/dev/null 2>&1

SRC_CFG="$TMP/gh/claude-config"
SRC_LIB="$TMP/gh/project-template/.claude"
[ -d "$SRC_CFG" ]          || { echo "HATA: $SRC_CFG bulunamadı"; exit 1; }
[ -d "$SRC_LIB/skills" ]   || { echo "HATA: skill kütüphanesi ($SRC_LIB/skills) bulunamadı"; exit 1; }

echo "→ Scaffold (manifest + script + şablon) kopyalanıyor…"
cp -r "$SRC_CFG/." ./

echo "→ Eski materyalize içerik temizleniyor (republish'te silmeleri yansıt)…"
rm -rf plugins/*/skills plugins/*/agents plugins/*/commands 2>/dev/null || true

echo "→ Plugin'ler materyalize ediliyor…"
bash scripts/build-marketplace.sh --source "$SRC_LIB"

echo "→ Commit + push…"
git add -A
git add -f plugins/                 # materyalize skill/agent (scaffold .gitignore'una rağmen yayımla)
if git diff --cached --quiet; then
  echo "ℹ Değişiklik yok — marketplace zaten güncel."; exit 0
fi
git commit -q -m "Mutfak plugin marketplace yayımlandı/güncellendi"
git push -u origin HEAD

echo "✔ Marketplace yayımlandı → mutfak-yazilimevi/claude-config"
echo "  Projelerde: /plugin marketplace add mutfak-yazilimevi/claude-config"
