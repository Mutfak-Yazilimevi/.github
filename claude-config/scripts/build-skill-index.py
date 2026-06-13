#!/usr/bin/env python3
# build-skill-index.py — Mutfak skill kütüphanesi için kategori bazlı ad
# indeksi (index_skills.md) üretir.
#
# Skill'leri TEK kaynaktan (Mutfak skill kütüphanesi .claude/skills) tarar,
# build-marketplace.sh ile BİREBİR aynı önek eşlemesini kullanarak plugin'e
# göre, hacimli/kümelenen kovalarda da skill adının 2. sözcüğüne göre
# alt-kategorilere ayırır. Yalnız ad listeler (açıklama yok); detay için
# ilgili SKILL.md'ye bakılır. Çıktı: <source>/skills/index_skills.md
#
# Kullanım:
#   python3 scripts/build-skill-index.py --source /path/to/.claude
#   (--source: skills/ içeren .claude dizini)
#
# Not: index_skills.md bir dizin olmadığından build-marketplace.sh
# tarafından materyalize EDİLMEZ (yalnız */ dizinleri kopyalanır).
import os
import sys
import fnmatch
import datetime

# --- argüman ---
src = ""
args = sys.argv[1:]
while args:
    a = args.pop(0)
    if a == "--source":
        src = args.pop(0) if args else ""
    else:
        sys.exit(f"bilinmeyen arg: {a}")
if not src:
    sys.exit("Kullanım: build-skill-index.py --source <.claude dizini>")
SK = os.path.join(src, "skills")
if not os.path.isdir(SK):
    sys.exit(f"HATA: {SK} yok")

# --- build-marketplace.sh skill_plugin() ile birebir (case sırası korunur) ---
PATTERNS = [
    ("mutfak-dotnet",    ["dev-dotnet-*"]),
    ("mutfak-dev",       ["dev-*"]),
    ("mutfak-frontend",  ["fe-*", "frontend", "web-*", "web", "theme*", "webapp*"]),
    ("mutfak-design",    ["design-*"]),
    ("mutfak-pm",        ["pm-*"]),
    ("mutfak-marketing", ["mkt-*", "seo*"]),
    ("mutfak-security",  ["sec-*"]),
    ("mutfak-research",  ["research-*"]),
    ("mutfak-diagrams",  ["md-*"]),
    ("mutfak-consulting", ["ali-*"]),
]
# index'te plugin sırası + kısa açıklama
PLUGIN_ORDER = [
    ("mutfak-security",   "Savunma/blue-team güvenlik (forensics, detection, hunting, hardening, pentest)"),
    ("mutfak-consulting", "İş/ops/C-level danışmanlık (ali-*)"),
    ("mutfak-dev",        "Genel yazılım geliştirme (TDD, review, debug, mimari, deploy, dil)"),
    ("mutfak-marketing",  "Pazarlama, SEO, growth, içerik"),
    ("mutfak-pm",         "Ürün yönetimi: strateji, PRD, metrik, keşif"),
    ("mutfak-design",     "Vendor marka/ürün tasarım sistemleri"),
    ("mutfak-dotnet",     ".NET test/diag/benchmark/migration skill'leri"),
    ("mutfak-diagrams",   "Markdown diyagram/görselleştirme"),
    ("mutfak-frontend",   "Frontend/web premium UI, generative UI, test"),
    ("mutfak-research",   "Akademik & derin araştırma"),
    ("mutfak-core",       "Çekirdek/genel (belge, görsel, artefakt, tekil)"),
]
# Yalnız adlandırması anlamlı kümelenen hacimli kovalar alt-başlığa ayrılır.
# Diğerleri düz alfabetik (adlar zaten önek bazlı yan yana kümelenir).
SUBGROUP = {"mutfak-security", "mutfak-consulting"}
# Alt-kategori en az bu kadar üye içermeli; aksi halde "Diğer"e toplanır.
MIN_GROUP = 3


def plugin_of(name):
    for pl, pats in PATTERNS:
        if any(fnmatch.fnmatch(name, p) for p in pats):
            return pl
    return "mutfak-core"


def subcat(name):
    parts = name.split("-")
    return parts[1] if len(parts) > 1 else "diğer"


names = sorted(d for d in os.listdir(SK) if os.path.isdir(os.path.join(SK, d)))
buckets = {pl: [] for pl, _ in PLUGIN_ORDER}
for n in names:
    buckets[plugin_of(n)].append(n)

total = len(names)
today = datetime.date.today().isoformat()

out = []
out.append("# Skill İndeksi\n")
out.append("> `project-template/.claude/skills/` altındaki tüm skill'lerin **kategori bazlı** ad listesi.")
out.append("> Plugin → alt-kategori şeklinde gruplandı (hacimli kovalarda skill adının 2. sözcüğüne göre).")
out.append("> Plugin eşlemesi `scripts/build-marketplace.sh` ile birebir aynıdır. Açıklama için ilgili `SKILL.md`'ye bakın.")
out.append(f"> Bu dosya `scripts/build-skill-index.py` ile üretilir — elle düzenlemeyin, yeniden çalıştırın.")
out.append(f"> **Toplam:** {total} skill · **Son tarama:** {today}\n")
toc = " · ".join(f"[{pl} ({len(buckets[pl])})](#{pl})"
                 for pl, _ in PLUGIN_ORDER if buckets[pl])
out.append(f"> **Plugin'ler:** {toc}\n")
out.append("---\n")

for pl, desc in PLUGIN_ORDER:
    items = buckets[pl]
    if not items:
        continue
    out.append(f"## {pl}\n")
    out.append(f"> {desc} · **{len(items)} skill**\n")
    if pl in SUBGROUP:
        groups = {}
        for n in items:
            groups.setdefault(subcat(n), []).append(n)
        multi = {k: v for k, v in groups.items() if len(v) >= MIN_GROUP}
        singles = [n for k, v in groups.items() if len(v) < MIN_GROUP for n in v]
        for k in sorted(multi, key=lambda k: (-len(multi[k]), k)):
            out.append(f"### {k.capitalize()} ({len(multi[k])})\n")
            out.append("\n".join(f"- `{n}`" for n in sorted(multi[k])) + "\n")
        if singles:
            out.append(f"### Diğer ({len(singles)})\n")
            out.append("\n".join(f"- `{n}`" for n in sorted(singles)) + "\n")
    else:
        out.append("\n".join(f"- `{n}`" for n in sorted(items)) + "\n")
    out.append("---\n")

dest = os.path.join(SK, "index_skills.md")
with open(dest, "w") as f:
    f.write("\n".join(out) + "\n")

print(f"✔ {dest} yazıldı — {total} skill")
for pl, _ in PLUGIN_ORDER:
    if buckets[pl]:
        print(f"  {pl:20s} {len(buckets[pl]):4d}")
