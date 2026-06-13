#!/usr/bin/env bash
# build-marketplace.sh — Mutfak Claude Code plugin marketplace'ini doldurur.
#
# Skill/agent'lar TEK kaynaktan (Mutfak skill kütüphanesi) gelir; bu script
# onları önek/eşleşmeye göre plugins/<ad>/{skills,agents}/ altına materyalize
# eder. Slash komutları (commands/) ise mutfak-core/commands/ altına kopyalanır
# (böylece /onboard, /intake, /review … plugin'le dağıtılır). Marketplace repo'su
# yalnız manifest + bu script tutar; ağır içerik build aninda gelir.
#
# NOT: rules/, hooks/, CLAUDE.md, .mcp.json plugin ile DAĞITILMAZ — bunlar proje
# scaffold'udur; tam kurulum için project-template kopyalanır.
#
# Kullanım:
#   bash scripts/build-marketplace.sh --source /path/to/.claude
#   (--source: kaynak .claude dizini; skills/ ve agents/ icermeli)
set -euo pipefail
cd "$(dirname "$0")/.."

SRC=""
while [ $# -gt 0 ]; do case "$1" in --source) SRC="$2"; shift 2;; *) echo "bilinmeyen arg: $1"; exit 1;; esac; done
[ -z "$SRC" ] && { echo "Kullanım: build-marketplace.sh --source <.claude dizini>"; exit 1; }
[ -d "$SRC/skills" ] || { echo "HATA: $SRC/skills yok"; exit 1; }

SK="$SRC/skills"; AG="$SRC/agents"

# sec- alt-plugin routing — TEK doğruluk kaynağı: skills/sec-routing.tsv
# (classify-security.py üretir). sec- skill'leri 6 güvenlik alt-plugin'ine yönlendirir;
# eşleşme yoksa mutfak-security-defense.
declare -A SEC_ROUTE
if [ -f "$SK/sec-routing.tsv" ]; then
  while IFS=$'\t' read -r _sn _sp; do [ -n "$_sn" ] && SEC_ROUTE["$_sn"]="$_sp"; done < "$SK/sec-routing.tsv"
fi

# Bir skill'i hangi plugin'e? (önek bazlı; sec- için routing tablosu)
skill_plugin() {
  case "$1" in
    dev-dotnet-*)                 echo mutfak-dotnet ;;
    dev-*)                        echo mutfak-dev ;;
    fe-*|frontend|web-*|web|theme*|webapp*) echo mutfak-frontend ;;
    design-*)                     echo mutfak-design ;;
    pm-*)                         echo mutfak-pm ;;
    mkt-*|seo*)                   echo mutfak-marketing ;;
    sec-*)                        echo "${SEC_ROUTE[$1]:-mutfak-security-defense}" ;;
    research-*)                   echo mutfak-research ;;
    md-*)                         echo mutfak-diagrams ;;
    ali-*)                        echo mutfak-consulting ;;
    *)                            echo mutfak-core ;;   # geri kalan tekil/anthropic
  esac
}

# Bir agent'ı hangi plugin'e?
agent_plugin() {
  case "$1" in
    akka-net-specialist|roslyn-*|dotnet-*|docfx-specialist) echo mutfak-dotnet ;;
    spec-*|agent-organizer|tech-lead-orchestrator|team-configurator|project-analyst) echo mutfak-spec-workflow ;;
    blog-*)                                                 echo mutfak-marketing ;;
    frontend-developer|senior-frontend-architect|react-pro|nextjs-pro|ui-designer|ux-designer|ui-ux-master) echo mutfak-frontend ;;
    security-auditor)                                       echo mutfak-security-grc ;;
    product-manager)                                        echo mutfak-pm ;;
    prompt-engineer)                                        echo mutfak-core ;;
    *)                                                      echo mutfak-dev ;;  # backend/data/devops/quality/dil uzmanları
  esac
}

echo "== Skill'ler materyalize ediliyor =="
declare -A scount
for d in "$SK"/*/; do
  [ -d "$d" ] || continue
  name=$(basename "$d"); pl=$(skill_plugin "$name")
  dest="plugins/$pl/skills"; mkdir -p "$dest"
  rm -rf "$dest/$name"; cp -r "$d" "$dest/$name"; rm -rf "$dest/$name/.git"
  scount[$pl]=$(( ${scount[$pl]:-0} + 1 ))
done

echo "== Agent'lar materyalize ediliyor =="
declare -A acount
if [ -d "$AG" ]; then
  for f in "$AG"/*.md; do
    [ -f "$f" ] || continue
    name=$(basename "$f" .md); [ "$name" = "index_agents" ] && continue
    pl=$(agent_plugin "$name")
    dest="plugins/$pl/agents"; mkdir -p "$dest"
    cp "$f" "$dest/"
    acount[$pl]=$(( ${acount[$pl]:-0} + 1 ))
  done
fi

echo "== Komutlar materyalize ediliyor (→ mutfak-core) =="
CMD="$SRC/commands"; ccount=0
if [ -d "$CMD" ]; then
  dest="plugins/mutfak-core/commands"; mkdir -p "$dest"
  for f in "$CMD"/*.md; do
    [ -f "$f" ] || continue
    cp "$f" "$dest/"; ccount=$(( ccount + 1 ))
  done
fi
echo "  $ccount komut → mutfak-core (/review, /onboard, /intake, /deploy …)"

echo
echo "== Özet (plugin: skill / agent) =="
for pl in $(ls plugins); do
  printf '  %-22s %4s skill  %3s agent\n' "$pl" "${scount[$pl]:-0}" "${acount[$pl]:-0}"
done
echo "  (komutlar: $ccount → mutfak-core/commands)"
echo "✔ Marketplace dolduruldu. (plugins/*/{skills,agents,commands} .gitignore'da; publish force-add eder.)"
