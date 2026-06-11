#!/usr/bin/env bash
# setup.sh — Lisanssız / koşullu / katalog kaynaklarını YEREL olarak çeker.
#
# Neden script? Bu kaynaklar ya lisanssız/CC ya da devasa (1.500+ skill) olduğu
# için repoya commit EDİLMEZ (.gitignore: _staging/, _catalog/). Bunun yerine
# kurulum sırasında bu script ile çekilirler — reproduce edilebilir, ama org
# reposunu şişirmez ve lisans riski taşımaz.
#
# Kullanım:  bash scripts/setup.sh [--staging] [--catalog] [--cybersec] [--all]
# Önkoşul:   git
set -euo pipefail
cd "$(dirname "$0")/.."

clone() { # url dest
  if [ -d "$2/.git" ]; then echo "↻ güncel: $2"; (cd "$2" && git pull --ff-only -q) || true
  else echo "⤓ klonlanıyor: $2"; git clone --depth 1 -q "$1" "$2"; fi
}

do_staging() {
  echo "== Faz 3.2 — koşullu / lisanssız → _staging/ (auto-load EDİLMEZ) =="
  clone https://github.com/master5d/claude-design-skills        _staging/master5d
  clone https://github.com/slavingia/skills                     _staging/slavingia
  clone https://github.com/markdown-viewer/skills               _staging/markdown-viewer
  clone https://github.com/ihlamury/design-skills               _staging/design-skills
  clone https://github.com/remotion-dev/skills                  _staging/remotion
  clone https://github.com/zhsama/claude-sub-agent              _staging/spec-workflow   # SA5
  echo "  ⚠️  Bunlar lisanssız/belirsiz — incelemeden ana .claude/'a TAŞIMA."
}

do_catalog() {
  echo "== Faz 3.3 — mega-kataloglar → _catalog/ (index, IMPORT YOK) =="
  clone https://github.com/ComposioHQ/awesome-claude-skills     _catalog/awesome-claude-skills
  clone https://github.com/ComposioHQ/awesome-codex-skills      _catalog/awesome-codex-skills
  clone https://github.com/sickn33/antigravity-awesome-skills   _catalog/antigravity-awesome-skills
  echo "  📚 Yalnız arama/index amaçlı; skill olarak import edilmez."
}

do_cybersec() {
  echo "== #31 — cybersec SAVUNMA alt kümesi → .claude/skills/sec-* =="
  src="$(mktemp -d)"
  clone https://github.com/mukul975/Anthropic-Cybersecurity-Skills "$src/cybersec"
  OFF='^exploiting-|attack-test|attack-simulation|-attack$|penetration-test|pentest|password-cracking|hash-cracking|with-metasploit|spearphishing|phishing-simulation|simulation-campaign|clickjacking-attack|cache-poisoning|request-smuggling|-hijacking|prototype-pollution|mass-assignment|policy-bypass|csp-bypass|-bypass$|bypass-|authentication-bypass|weaponiz|red-team|broken-function-level|broken-object|oauth-misconfiguration|smb-vulnerabilities'
  n=0
  while IFS= read -r f; do
    d=$(dirname "$f"); base=$(basename "$d")
    echo "$base" | grep -iqE "$OFF" && continue       # saldırgan → atla
    cp -r "$d" ".claude/skills/sec-$base"; rm -rf ".claude/skills/sec-$base/.git"; n=$((n+1))
  done < <(find "$src/cybersec" -name SKILL.md)
  rm -rf "$src"
  echo "  ✓ $n savunma skill eklendi (saldırgan/exploit filtrelendi)."
}

case "${1:-}" in
  --staging)  do_staging ;;
  --catalog)  do_catalog ;;
  --cybersec) do_cybersec ;;
  --all)      do_staging; do_catalog; do_cybersec ;;
  *) echo "Kullanım: bash scripts/setup.sh [--staging|--catalog|--cybersec|--all]"; exit 1 ;;
esac
echo "✔ Bitti."
