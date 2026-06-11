#!/usr/bin/env bash
# build-plugins.sh — Faz 7 plugin'lerini .claude/ içeriğinden materyalize eder.
#
# Skill/agent içeriği repoda tek yerde (.claude/skills, .claude/agents) tutulur;
# plugin'ler dağıtım anında buradan kopyalanarak üretilir → repo şişmez.
#
# Kullanım:  bash scripts/build-plugins.sh [dotnet-pack|frontend-pack|marketing-pack|all]
set -euo pipefail
cd "$(dirname "$0")/.."

SK=.claude/skills
AG=.claude/agents

copy_skills() { # plugin  prefix...
  local plugin="$1"; shift
  local dest="plugins/$plugin/skills"; mkdir -p "$dest"
  local n=0
  for pref in "$@"; do
    for d in $SK/$pref*/; do
      [ -d "$d" ] || continue
      cp -r "$d" "$dest/$(basename "$d")"; n=$((n+1))
    done
  done
  echo "  $plugin: $n skill materyalize edildi → $dest"
}

copy_agents() { # plugin  agent-file...
  local plugin="$1"; shift
  local dest="plugins/$plugin/agents"; mkdir -p "$dest"
  local n=0
  for a in "$@"; do
    [ -f "$AG/$a" ] && cp "$AG/$a" "$dest/" && n=$((n+1))
  done
  echo "  $plugin: $n agent materyalize edildi → $dest"
}

build_dotnet() {
  echo "▶ dotnet-pack"
  copy_skills dotnet-pack "dev-dotnet-"
  copy_agents dotnet-pack \
    akka-net-specialist.md roslyn-incremental-generator-specialist.md \
    dotnet-concurrency-specialist.md dotnet-performance-analyst.md \
    dotnet-benchmark-designer.md docfx-specialist.md
}
build_frontend() { echo "▶ frontend-pack"; copy_skills frontend-pack "fe-"; }
build_marketing(){ echo "▶ marketing-pack"; copy_skills marketing-pack "mkt-" "seo-technical"; }

case "${1:-all}" in
  dotnet-pack)    build_dotnet ;;
  frontend-pack)  build_frontend ;;
  marketing-pack) build_marketing ;;
  all)            build_dotnet; build_frontend; build_marketing ;;
  *) echo "Kullanım: bash scripts/build-plugins.sh [dotnet-pack|frontend-pack|marketing-pack|all]"; exit 1 ;;
esac
echo "✔ Plugin(ler) hazır. Not: plugins/*/skills ve plugins/*/agents .gitignore'da — dagitimda uretilir."
