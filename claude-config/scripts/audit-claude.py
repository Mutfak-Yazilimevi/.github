#!/usr/bin/env python3
# audit-claude.py — .claude kütüphanesinde hatalı durumları SALT-OKUNUR tespit eder.
# Kontroller: skill/agent frontmatter (name/description/model), agent name==dosya,
# aynı-plugin içi çakışan frontmatter name, settings.json + hook yolları, .mcp.json,
# dokümanlarda kırık skill/agent referansı, katalog↔disk senkronu.
# Çıkış kodu: bulgu yoksa 0, ERR varsa 1.
#
# Kullanım: python3 scripts/audit-claude.py --source <.claude dizini> [--repo-root <yol>]
import os, re, sys, json, csv, glob, fnmatch
from collections import Counter, defaultdict

src=""; root=""
a=sys.argv[1:]
while a:
    x=a.pop(0)
    if x=="--source": src=a.pop(0) if a else ""
    elif x=="--repo-root": root=a.pop(0) if a else ""
    else: sys.exit(f"bilinmeyen arg: {x}")
if not src: sys.exit("Kullanım: audit-claude.py --source <.claude dizini> [--repo-root <yol>]")
CL=src; SK=f"{CL}/skills"; AG=f"{CL}/agents"
if not root: root=os.path.dirname(os.path.abspath(CL)) or "."

issues=[]
def add(sev,cat,msg): issues.append((sev,cat,msg))

SKILL_PATTERNS=[("mutfak-dotnet",["dev-dotnet-*"]),("mutfak-dev",["dev-*"]),
 ("mutfak-frontend",["fe-*","frontend","web-*","web","theme*","webapp*"]),("mutfak-design",["design-*"]),
 ("mutfak-pm",["pm-*"]),("mutfak-marketing",["mkt-*","seo*"]),("mutfak-security",["sec-*"]),
 ("mutfak-research",["research-*"]),("mutfak-diagrams",["md-*"]),("mutfak-consulting",["ali-*"])]
def skill_plugin(n):
    for pl,ps in SKILL_PATTERNS:
        if any(fnmatch.fnmatch(n,p) for p in ps): return pl
    return "mutfak-core"

def frontmatter(path):
    try: raw=open(path,encoding="utf-8").read()
    except: return None
    m=re.match(r"^﻿?---\s*\n(.*?)\n---",raw,re.DOTALL)
    if not m: return {}
    fm={}
    for line in m.group(1).split("\n"):
        km=re.match(r"^([A-Za-z_][\w-]*):\s?(.*)$",line)
        if km: fm[km.group(1)]=km.group(2).strip().strip('"').strip("'")
    return fm

skill_dirs=sorted(d for d in os.listdir(SK) if os.path.isdir(f"{SK}/{d}"))
agent_files=sorted(f[:-3] for f in os.listdir(AG) if f.endswith(".md") and f!="index_agents.md")
valid=set(skill_dirs)|set(agent_files)

names_by_plugin=defaultdict(lambda: defaultdict(list))
for d in skill_dirs:
    sp=f"{SK}/{d}/SKILL.md"
    if not os.path.isfile(sp): add("ERR","skill",f"{d}: SKILL.md yok"); continue
    fm=frontmatter(sp)
    if not fm: add("ERR","skill",f"{d}: frontmatter yok/bozuk"); continue
    if not fm.get("name"): add("ERR","skill",f"{d}: name boş")
    if not fm.get("description"): add("ERR","skill",f"{d}: description boş")
    # iç içe SKILL.md — assets/ hariç (test fixture'ları)
    nested=[x for x in glob.glob(f"{SK}/{d}/**/SKILL.md",recursive=True)
            if x!=sp and "/assets/" not in x and "/references/" not in x]
    if nested: add("WARN","skill",f"{d}: keşfedilmeyen iç içe SKILL.md: {nested}")
    if fm.get("name"): names_by_plugin[skill_plugin(d)][fm["name"]].append(d)
# aynı-plugin içi çakışan name = ERR
for pl,nm in names_by_plugin.items():
    for name,ds in nm.items():
        if len(ds)>1: add("ERR","collision",f"[{pl}] frontmatter name '{name}' çakışması: {ds}")

for x in os.listdir(SK):
    if not os.path.isdir(f"{SK}/{x}") and x not in ("index_skills.md","skills-catalog.csv"):
        add("WARN","skill",f"skills/ kökünde beklenmedik dosya: {x}")

VALID_MODELS={"haiku","sonnet","opus"}
for ag in agent_files:
    fm=frontmatter(f"{AG}/{ag}.md")
    if not fm: add("ERR","agent",f"{ag}: frontmatter yok"); continue
    if fm.get("name")!=ag: add("ERR","agent",f"{ag}: name='{fm.get('name')}' ≠ dosya adı")
    if fm.get("model") not in VALID_MODELS: add("WARN","agent",f"{ag}: model='{fm.get('model')}'")
    if not fm.get("description"): add("WARN","agent",f"{ag}: description boş")

try:
    s=json.load(open(f"{CL}/settings.json"))
    for ev,arr in s.get("hooks",{}).items():
        for grp in arr:
            for h in grp.get("hooks",[]):
                f=(h.get("command","").split() or [""])[0]
                if f.startswith(".claude/"):
                    fp=os.path.join(root,f)
                    if not os.path.isfile(fp): add("ERR","hook",f"{ev}: komut dosyası yok: {f}")
                    elif not os.access(fp,os.X_OK): add("WARN","hook",f"{ev}: {f} çalıştırılabilir değil")
except Exception as e: add("ERR","settings",f"settings.json: {e}")

mcp=os.path.join(root,".mcp.json")
if os.path.isfile(mcp):
    try: json.load(open(mcp))
    except Exception as e: add("ERR","mcp",f".mcp.json: {e}")

DOCS=glob.glob(f"{CL}/rules/*.md")+glob.glob(f"{CL}/commands/*.md")+\
     [os.path.join(root,"CLAUDE.md"),f"{CL}/README.md"]+\
     glob.glob(f"{SK}/dev-*/SKILL.md")
ref_re=re.compile(r"`((?:dev|ali|pm|mkt|sec|design|md|fe|web|research|seo)-[a-z0-9][a-z0-9-]+)`")
for doc in DOCS:
    if not os.path.isfile(doc): continue
    for tok in set(ref_re.findall(open(doc,encoding="utf-8").read())):
        if tok.endswith("-") or "*" in tok: continue
        if tok not in valid: add("WARN","ref",f"{os.path.relpath(doc,root)}: '{tok}' referansı YOK")

def catnames(p):
    try: return [r[0] for r in list(csv.reader(open(p,encoding="utf-8")))[1:]]
    except: return None
sc=catnames(f"{SK}/skills-catalog.csv")
if sc is None: add("WARN","catalog","skills-catalog.csv yok/okunamadı (build-catalog.py?)")
else:
    if set(skill_dirs)-set(sc): add("ERR","catalog",f"katalogda eksik skill: {sorted(set(skill_dirs)-set(sc))[:5]}")
    if set(sc)-set(skill_dirs): add("ERR","catalog",f"katalogda fazla skill: {sorted(set(sc)-set(skill_dirs))[:5]}")

c=Counter(s for s,_,_ in issues)
print(f"Taranan: {len(skill_dirs)} skill, {len(agent_files)} agent")
print(f"BULGULAR: {c.get('ERR',0)} ERR, {c.get('WARN',0)} WARN\n")
for sev in ("ERR","WARN"):
    for s,cat,msg in issues:
        if s==sev: print(f"  [{sev}/{cat}] {msg}")
if not issues: print("  ✓ Hata bulunamadı.")
sys.exit(1 if c.get("ERR") else 0)
