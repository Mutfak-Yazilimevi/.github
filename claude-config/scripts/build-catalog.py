#!/usr/bin/env python3
# build-catalog.py — Skill ve agent'lar için HIZLI SORGULANABİLİR katalog (CSV) üretir.
#
# Her SKILL.md / agent .md frontmatter'ını (name, description, model) tarar ve
# zengin kolonlu CSV'ler yazar:
#   <source>/skills/skills-catalog.csv  → name, category, subcategory, tech, path, description
#   <source>/agents/agents-catalog.csv  → name, category, model, tech, path, description
#
# Kolonlar grep/jq/awk ile aranabilir; "ada/amaca/kategoriye/teknolojiye göre hızlı bul".
# Plugin (category) eşlemesi build-marketplace.sh ile birebir aynıdır.
#
# Kullanım: python3 scripts/build-catalog.py --source <.claude dizini> [--repo-prefix project-template/.claude]
#
# Not: .csv dosyaları build-marketplace.sh tarafından materyalize EDİLMEZ
# (skill'ler */ dizinleri, agent'lar *.md ile kopyalanır; .csv ikisine de uymaz).
import os
import re
import sys
import csv
import fnmatch

src = ""
repo_prefix = "project-template/.claude"
args = sys.argv[1:]
while args:
    a = args.pop(0)
    if a == "--source":
        src = args.pop(0) if args else ""
    elif a == "--repo-prefix":
        repo_prefix = args.pop(0) if args else ""
    else:
        sys.exit(f"bilinmeyen arg: {a}")
if not src:
    sys.exit("Kullanım: build-catalog.py --source <.claude dizini> [--repo-prefix <yol>]")
SK = os.path.join(src, "skills")
AG = os.path.join(src, "agents")

# --- skill önek → plugin (build-marketplace.sh / build-skill-index.py ile birebir) ---
SKILL_PATTERNS = [
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


def skill_plugin(name):
    for pl, pats in SKILL_PATTERNS:
        if any(fnmatch.fnmatch(name, p) for p in pats):
            return pl
    return "mutfak-core"


def agent_plugin(name):
    if re.match(r"akka-net-specialist|roslyn-|dotnet-|docfx-specialist", name):
        return "mutfak-dotnet"
    if re.match(r"spec-", name) or name in (
        "agent-organizer", "tech-lead-orchestrator", "team-configurator", "project-analyst"):
        return "mutfak-spec-workflow"
    if name.startswith("blog-"):
        return "mutfak-marketing"
    if name in ("frontend-developer", "senior-frontend-architect", "react-pro", "nextjs-pro",
                "ui-designer", "ux-designer", "ui-ux-master"):
        return "mutfak-frontend"
    if name == "security-auditor":
        return "mutfak-security"
    if name == "product-manager":
        return "mutfak-pm"
    if name == "prompt-engineer":
        return "mutfak-core"
    return "mutfak-dev"


# --- teknoloji etiketi (heuristik: ad+açıklama içinde anahtar kelime) ---
TECH = [
    (".NET", [r"\.net\b", r"\bdotnet", r"\bc#", r"\bcsharp", r"asp\.?net", r"\bef core", r"roslyn", r"akka\.net", r"blazor"]),
    ("TypeScript", [r"typescript"]), ("JavaScript", [r"javascript", r"\bnode\.?js"]),
    ("React", [r"\breact\b"]), ("Next.js", [r"next\.?js"]), ("Vue", [r"\bvue"]), ("Astro", [r"\bastro\b"]),
    ("Python", [r"\bpython\b"]), ("Go", [r"\bgolang\b", r"\bgo\b"]), ("Rust", [r"\brust\b"]),
    ("Java", [r"\bjava\b"]), ("Kotlin", [r"kotlin"]), ("Swift", [r"\bswift\b"]),
    ("Flutter", [r"flutter"]), ("React Native", [r"react native"]), ("Electron", [r"electron"]),
    ("PostgreSQL", [r"postgres", r"pglite", r"npgsql"]), ("SQL", [r"\bsql\b"]), ("Redis", [r"\bredis"]),
    ("Kafka", [r"kafka"]), ("Spark", [r"\bspark\b"]), ("Airflow", [r"airflow"]),
    ("AWS", [r"\baws\b", r"lambda", r"\bs3\b", r"cloudtrail", r"guardduty"]), ("Azure", [r"\bazure\b"]),
    ("GCP", [r"\bgcp\b", r"google cloud"]), ("Kubernetes", [r"kubernetes", r"\bk8s\b"]),
    ("Docker", [r"docker", r"container"]), ("Terraform", [r"terraform"]),
    ("GraphQL", [r"graphql"]), ("gRPC", [r"grpc"]), ("REST", [r"\brest\b", r"restful"]),
    ("LLM", [r"\bllm\b", r"prompt", r"\brag\b", r"agentic", r"vector"]), ("MCP", [r"\bmcp\b", r"model context protocol"]),
    ("Figma", [r"figma"]), ("Playwright", [r"playwright"]), ("Cloudflare", [r"cloudflare", r"workers"]),
    ("Splunk", [r"splunk"]), ("Zeek", [r"\bzeek\b"]), ("YARA", [r"\byara\b"]), ("MITRE", [r"mitre", r"att&ck"]),
    ("SEO", [r"\bseo\b"]), ("Stripe", [r"stripe"]),
]


def tech_tags(text):
    t = text.lower()
    tags = [disp for disp, pats in TECH if any(re.search(p, t) for p in pats)]
    return ";".join(dict.fromkeys(tags))


# --- frontmatter parse (yaml stdlib yok; tolerant) ---
def parse_frontmatter(path):
    out = {"name": "", "description": "", "model": ""}
    try:
        raw = open(path, encoding="utf-8").read()
    except Exception:
        return out
    m = re.match(r"^﻿?---\s*\n(.*?)\n---", raw, re.DOTALL)
    if not m:
        return out
    fm = m.group(1)
    # blok scalar ve çok-satırlı description'ı tek satıra indir
    lines = fm.split("\n")
    i = 0
    while i < len(lines):
        line = lines[i]
        km = re.match(r"^([A-Za-z_][\w-]*):\s?(.*)$", line)
        if km:
            key, val = km.group(1), km.group(2)
            if val in (">", "|", ">-", "|-", ">+", "|+") or val == "":
                # devam eden girintili satırları topla
                buf = []
                j = i + 1
                while j < len(lines) and (lines[j].startswith((" ", "\t")) or lines[j].strip() == ""):
                    buf.append(lines[j].strip())
                    j += 1
                val = " ".join(x for x in buf if x)
                i = j - 1
            val = val.strip().strip('"').strip("'").strip()
            if key in out:
                out[key] = re.sub(r"\s+", " ", val)
        i += 1
    return out


def subcat(name):
    parts = name.split("-")
    return parts[1] if len(parts) > 1 else ""


# --- SKILLS ---
skill_rows = []
if os.path.isdir(SK):
    for d in sorted(os.listdir(SK)):
        sp = os.path.join(SK, d, "SKILL.md")
        if not os.path.isfile(sp):
            continue
        fm = parse_frontmatter(sp)
        desc = fm["description"]
        skill_rows.append({
            "name": d,
            "category": skill_plugin(d),
            "subcategory": subcat(d),
            "tech": tech_tags(d + " " + desc),
            "path": f"{repo_prefix}/skills/{d}/SKILL.md",
            "description": desc,
        })

# --- AGENTS ---
agent_rows = []
if os.path.isdir(AG):
    for f in sorted(os.listdir(AG)):
        if not f.endswith(".md") or f == "index_agents.md":
            continue
        name = f[:-3]
        fm = parse_frontmatter(os.path.join(AG, f))
        desc = fm["description"]
        agent_rows.append({
            "name": fm["name"] or name,
            "category": agent_plugin(name),
            "model": fm["model"],
            "tech": tech_tags(name + " " + desc),
            "path": f"{repo_prefix}/agents/{f}",
            "description": desc,
        })


def write_csv(path, rows, cols):
    with open(path, "w", encoding="utf-8", newline="") as fh:
        w = csv.DictWriter(fh, fieldnames=cols, quoting=csv.QUOTE_MINIMAL)
        w.writeheader()
        for r in rows:
            w.writerow(r)


sc = os.path.join(SK, "skills-catalog.csv")
ac = os.path.join(AG, "agents-catalog.csv")
write_csv(sc, skill_rows, ["name", "category", "subcategory", "tech", "path", "description"])
write_csv(ac, agent_rows, ["name", "category", "model", "tech", "path", "description"])

print(f"✔ {sc} — {len(skill_rows)} skill")
print(f"✔ {ac} — {len(agent_rows)} agent")
