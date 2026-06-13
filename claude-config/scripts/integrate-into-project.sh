#!/usr/bin/env bash
# integrate-into-project.sh — Mutfak plugin marketplace'ini MEVCUT bir projeye
# güvenli biçimde entegre eder. Proje `.claude/` taşısa da taşımasa da çalışır;
# var olan `settings.json` / `CLAUDE.md` ASLA ezilmez — yalnız eksik anahtarlar eklenir.
#
# Ne yapar:
#   • <hedef>/.claude/ yoksa oluşturur.
#   • <hedef>/.claude/settings.json:
#       - yoksa: marketplace + seçili plugin'lerle oluşturur.
#       - varsa: JSON-merge — `extraKnownMarketplaces.mutfak` ve `enabledPlugins`
#         anahtarlarını ekler; diğer anahtarları (hooks, permissions, env, …) korur.
#         (enabledPlugins object formatında: "plugin@mutfak": true)
#   • <hedef>/CLAUDE.md yoksa (ve --no-claude-md verilmediyse) kısa bir yönlendirme stub'ı yazar.
#   • İdempotent: tekrar çalıştırınca değişiklik yoksa dosyaya dokunmaz.
#
# Kullanım:
#   bash integrate-into-project.sh [--target DIR] [--profile AD | --plugins a,b,c]
#                                  [--marketplace-repo owner/repo] [--no-claude-md] [--dry-run]
#
#   --target   : hedef proje kökü (varsayılan: cwd)
#   --profile  : hazır profil (aşağıdaki liste; bkz. templates/PROFILES.md)
#   --plugins  : virgül ayrımlı plugin kısa adları (core,dev,dotnet,...) — --profile yerine
#   --dry-run  : değişiklikleri yazmadan göster
set -euo pipefail

TARGET="$PWD"
PROFILE=""
PLUGINS_CSV=""
MARKET_REPO="mutfak-yazilimevi/claude-config"
MARKET_NAME="mutfak"
WRITE_CLAUDE_MD=1
DRY_RUN=0

# --- profil → plugin kısa adları (templates/PROFILES.md ile uyumlu) ---
profile_plugins() {
  case "$1" in
    minimal)       echo "core dev" ;;
    dotnet-api)    echo "core dev dotnet security" ;;
    web-frontend)  echo "core dev frontend design" ;;
    fullstack)     echo "core dev dotnet frontend design security" ;;
    mobile)        echo "core dev frontend" ;;
    marketing)     echo "core marketing pm design" ;;
    product)       echo "core pm consulting" ;;
    security)      echo "core dev security" ;;
    research)      echo "core research diagrams" ;;
    spec)          echo "core dev spec-workflow" ;;
    *) return 1 ;;
  esac
}
PROFILES="minimal dotnet-api web-frontend fullstack mobile marketing product security research spec"

usage() { sed -n '2,30p' "$0"; echo; echo "Profiller: $PROFILES"; }

while [ $# -gt 0 ]; do
  case "$1" in
    --target)            TARGET="$2"; shift 2 ;;
    --profile)           PROFILE="$2"; shift 2 ;;
    --plugins)           PLUGINS_CSV="$2"; shift 2 ;;
    --marketplace-repo)  MARKET_REPO="$2"; shift 2 ;;
    --no-claude-md)      WRITE_CLAUDE_MD=0; shift ;;
    --dry-run)           DRY_RUN=1; shift ;;
    -h|--help)           usage; exit 0 ;;
    *) echo "Bilinmeyen arg: $1"; usage; exit 1 ;;
  esac
done

[ -d "$TARGET" ] || { echo "HATA: hedef dizin yok: $TARGET"; exit 1; }
command -v python3 >/dev/null || { echo "HATA: python3 gerekli (JSON merge için)"; exit 1; }

# --- plugin listesini belirle ---
if [ -n "$PLUGINS_CSV" ]; then
  SHORTS="$(echo "$PLUGINS_CSV" | tr ',' ' ')"
elif [ -n "$PROFILE" ]; then
  SHORTS="$(profile_plugins "$PROFILE")" || { echo "HATA: bilinmeyen profil '$PROFILE'"; echo "Profiller: $PROFILES"; exit 1; }
else
  echo "Profil veya plugin verilmedi → 'minimal' (core, dev) kullanılıyor."
  echo "  Diğer profiller: $PROFILES   (örn: --profile dotnet-api)"
  SHORTS="$(profile_plugins minimal)"
fi

# kısa ad → tam plugin id (mutfak-<ad>@<market>)
PLUGIN_IDS=""
for s in $SHORTS; do
  PLUGIN_IDS="$PLUGIN_IDS mutfak-${s}@${MARKET_NAME}"
done

CLAUDE_DIR="$TARGET/.claude"
SETTINGS="$CLAUDE_DIR/settings.json"

echo "→ Hedef:        $TARGET"
echo "→ Marketplace:  $MARKET_NAME = $MARKET_REPO"
echo "→ Plugin'ler:   $(echo $PLUGIN_IDS | xargs)"
[ "$DRY_RUN" = 1 ] && echo "→ (dry-run: dosya yazılmayacak)"

# --- JSON merge (python; var olan anahtarları korur, idempotent) ---
MERGE_REPORT="$(
  PLUGIN_IDS="$PLUGIN_IDS" MARKET_NAME="$MARKET_NAME" MARKET_REPO="$MARKET_REPO" \
  SETTINGS="$SETTINGS" DRY_RUN="$DRY_RUN" python3 - <<'PY'
import json, os, sys

settings_path = os.environ["SETTINGS"]
ids = os.environ["PLUGIN_IDS"].split()
mname = os.environ["MARKET_NAME"]
mrepo = os.environ["MARKET_REPO"]
dry = os.environ["DRY_RUN"] == "1"

data, existed, parse_warn = {}, False, ""
if os.path.exists(settings_path):
    existed = True
    raw = open(settings_path, encoding="utf-8").read()
    try:
        data = json.loads(raw) if raw.strip() else {}
    except json.JSONDecodeError as e:
        print(f"PARSE_ERROR::settings.json geçersiz JSON ({e}); elle düzeltin, dokunulmadı.")
        sys.exit(2)
    if not isinstance(data, dict):
        print("PARSE_ERROR::settings.json kök object değil; dokunulmadı.")
        sys.exit(2)

before = json.dumps(data, sort_keys=True)
changes = []

# 1) extraKnownMarketplaces
ekm = data.setdefault("extraKnownMarketplaces", {})
if not isinstance(ekm, dict):
    print("PARSE_ERROR::extraKnownMarketplaces object değil; dokunulmadı.")
    sys.exit(2)
desired_src = {"source": {"source": "github", "repo": mrepo}}
if mname not in ekm:
    ekm[mname] = desired_src
    changes.append(f"marketplace '{mname}' eklendi")
elif ekm[mname] != desired_src:
    changes.append(f"marketplace '{mname}' zaten var (farklı tanım — korundu)")

# 2) enabledPlugins (object formuna normalize et)
ep = data.get("enabledPlugins")
if ep is None:
    ep = {}
elif isinstance(ep, list):  # eski array formu → object'e çevir
    ep = {k: True for k in ep}
    changes.append("enabledPlugins array → object formuna normalize edildi")
elif not isinstance(ep, dict):
    print("PARSE_ERROR::enabledPlugins beklenmeyen tip; dokunulmadı.")
    sys.exit(2)
for pid in ids:
    if ep.get(pid) is not True:
        ep[pid] = True
        changes.append(f"plugin etkin: {pid}")
data["enabledPlugins"] = ep

after = json.dumps(data, sort_keys=True)
changed = before != after

if changed and not dry:
    if existed:
        open(settings_path + ".bak", "w", encoding="utf-8").write(open(settings_path, encoding="utf-8").read())
    os.makedirs(os.path.dirname(settings_path), exist_ok=True)
    with open(settings_path, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
        f.write("\n")

status = "OLUSTURULDU" if not existed else ("DEGISTI" if changed else "DEGISIKLIK_YOK")
print(f"STATUS::{status}")
for c in changes:
    print(f"CHANGE::{c}")
PY
)" || {
  echo "$MERGE_REPORT" | sed 's/^PARSE_ERROR:://'
  exit 1
}

echo "$MERGE_REPORT" | sed -n 's/^STATUS::/settings.json: /p'
echo "$MERGE_REPORT" | sed -n 's/^CHANGE::/   • /p'
[ -f "$SETTINGS.bak" ] && echo "   (yedek: $SETTINGS.bak)"

# --- CLAUDE.md (yoksa kısa stub; varsa dokunma) ---
CMD="$TARGET/CLAUDE.md"
if [ "$WRITE_CLAUDE_MD" = 1 ] && [ ! -f "$CMD" ]; then
  if [ "$DRY_RUN" = 1 ]; then
    echo "CLAUDE.md: yok → oluşturulacak (dry-run)"
  else
    cat > "$CMD" <<EOF
# Project: $(basename "$TARGET")

<Bir-iki cümle: proje ne yapar, mimari yaklaşım.>

> Skill & agent'lar merkezi marketplace'ten gelir (\`$MARKET_REPO\`).
> Bu proje yalnız gerekli plugin'leri etkinleştirir — bkz. \`.claude/settings.json\` → \`enabledPlugins\`.
> Tüm kütüphaneyi projeye kopyalamaya gerek yok. Kullanım kılavuzu: marketplace reposundaki
> \`project-template/.claude/README.md\`.

## Tech Stack
- <runtime / DB / mimari>

## Commands
- <build / test / run>

## Conventions
- <proje-özel kurallar>
- Etkin plugin setine göre skill'ler otomatik tetiklenir (progressive disclosure — düşük token).
EOF
    echo "CLAUDE.md: oluşturuldu (stub — projeye göre doldur)"
  fi
else
  [ -f "$CMD" ] && echo "CLAUDE.md: zaten var → dokunulmadı"
fi

echo "✔ Entegrasyon tamam. Projeyi Claude Code'da açınca plugin'ler marketplace'ten çekilir."
