---
name: existing-project-onboarding
description: Read-only front door for an EXISTING project. Instead of asking the user, find the answers IN the codebase — understand what the project is, extract what is built vs what is missing (gaps), record ideas and opportunities to a backlog, explain a plan, and WAIT for an explicit directive before producing or binding a PRD. Makes NO source or config changes; the only files it writes are the backlog and an analysis report. Use to onboard onto an unfamiliar repo or assess an existing project before any work is planned.
---

# Existing Project Onboarding (read-only front door)

## Overview

For an existing codebase, the requirements already live **in the code**. This skill answers the
intake questions by *reading*, not asking: understand the project, separate done from missing,
capture ideas to a backlog, present a plan — then **stop and wait for a directive** before
turning anything into a PRD or work.

> **Read-only contract.** Make **no** changes to source, config, schema, or build. The ONLY
> files you may create/append are: `docs/backlog.md` (ideas) and an analysis report
> (`docs/onboarding-report.md`). Do not edit, refactor, fix, or "improve" anything. Do not run
> commands that mutate state (no migrations, installs that change lockfiles, formatters, git
> writes). Reading, building/tests in a throwaway sense only if explicitly safe.

## Workflow

0. **Envanter ÖNCE (zorunlu).** Herhangi bir yorum yapmadan önce **dosya ağacını gerçekten listele**
   ve dosya sayısını raporla:
   ```bash
   # gürültüyü ele (node_modules/bin/obj/.git), gizli dosyalar dahil
   find . -type f -not -path '*/node_modules/*' -not -path '*/.git/*' \
     -not -path '*/bin/*' -not -path '*/obj/*' | head -300
   find . -type f -not -path '*/.git/*' | wc -l   # toplam sayı
   ```
   veya `Glob **/*`. **Kanıt (liste/sayı) göstermeden "proje boş" DEME.** "Boş" görünüyorsa önce
   şunu kontrol et: doğru dizinde misin (`pwd`)? · dosyalar gizli/gitignored mı? · Claude Code üst
   bir klasörde mi açıldı? Gerçekten boşsa bunu listeyle kanıtla.
1. **Önce `.md` dosyaları (dokümanı oku).** Kod okumadan önce tüm markdown'ları bul ve **oku** —
   proje çoğu zaman burada anlatılır:
   ```bash
   find . -iname '*.md' -not -path '*/node_modules/*' -not -path '*/.git/*'
   ```
   Öncelik sırası: kök `README.md` → `CLAUDE.md` → `docs/**.md` → diğer `*.md` (CONTRIBUTING,
   ADR, changelog…). Bunlardan amacı, stack'i, mimariyi ve "nasıl çalışır"ı çıkar.
2. **Sonra kod & yapı.** Şimdi manifestleri (package.json, *.csproj, *.sln, requirements.txt…),
   giriş noktalarını, klasör yapısını ve testleri **oku**. Belirle: feature'lar, entegrasyonlar,
   katmanlar. Büyük repoda `Explore`/`Agent` ile (read-only) yay; `ali-codebase-onboarding` tarzı oku.
3. **Done vs missing.** Build two lists:
   - **Yapılanlar (done):** implemented features, covered tests, working flows.
   - **Eksikler/gaps:** missing features, TODO/FIXME, untested paths, broken/half-done areas,
     stale deps, security/architecture smells, missing docs. Cite `file:line` as evidence.
   Infer intended-vs-implemented (`pm-intended-vs-implemented` lens).
4. **Record ideas → backlog.** Append your findings and opportunities to `docs/backlog.md`
   (create from template if absent) as prioritized items — **ideas only, not changes**.
5. **Explain the plan.** Present: what the project is, the done/missing summary, the backlog you
   wrote, and a proposed sequence of work (with rationale and risks). Surface open questions.
6. **WAIT for a directive.** Do **not** create a PRD, start work, or modify code. Stop here and
   ask the user which backlog items to take forward. Only on an explicit directive do you bind
   selected items to a PRD (`pm-create-prd` / `dev-to-prd`) and enter `rules/process.md`.

## Output (the only writes allowed)

- **`docs/onboarding-report.md`** — purpose, stack/architecture, done list, gaps list (with
  `file:line`), risks, open questions.
- **`docs/backlog.md`** — prioritized ideas/opportunities (id, başlık, değer, efor, kanıt).
- **A chat summary** ending with: "Backlog hazır + plan açıklandı. PRD'ye bağlamam için hangi
  maddeleri seçiyorsunuz? (Direktif bekliyorum.)"

## Gotchas

- **Kanıtsız "boş proje"** — dosya ağacını listelemeden "proje boş / okuyacak bir şey yok" deme.
  Çoğu zaman yanlış cwd, üst klasörde açılma veya gizli/gitignored dosyalardandır. Önce `find`/`Glob`
  ile listele + sayı ver; gerçekten boşsa listeyle kanıtla.
- **Touching the code** — this skill is read-only; even a "tiny fix" violates the contract.
  If something is broken, record it in the backlog, don't fix it.
- **Assuming intent** — when the code is ambiguous, log an open question; don't invent purpose.
- **Skipping the wait** — never auto-create a PRD or start work; the directive gate is mandatory.
- **Mutating commands** — no installs/migrations/formatters/git writes during analysis.

## Related — ne zaman hangisi

Bu skill **salt-okunur orkestratördür** (anla → yapılan/eksik → backlog → dur). Aşağıdaki tekil
yetenekleri analiz sırasında (read-only) kullan veya direktiften sonra devreye sok:

**Kodu anlama (analiz sırasında, read-only):**
| İhtiyaç | Kullan |
| :--- | :--- |
| Kod tabanına onboarding / genel kavrayış | `ali-codebase-onboarding` |
| Rehberli kod turu | `ali-code-tour` |
| Monorepo / büyük repo gezinme | `ali-monorepo-navigator` |
| Bağlam derleme | `dev-context-engineering` · `ali-context-engine` |

**Yapılan vs eksik / boşluk (analiz sırasında, read-only):**
| İhtiyaç | Kullan |
| :--- | :--- |
| Amaçlanan vs uygulanan farkı | `pm-intended-vs-implemented` |
| Teknik borç tespiti | `ali-tech-debt-tracker` |
| Bağımlılık denetimi (eski/riskli) | `ali-dependency-auditor` |
| Kalite/koku analizi | `dev-code-review-and-quality` |
| Güvenlik sahiplik/boşluk | `dev-security-ownership-map` |
| Özellik talepleri → backlog | `pm-analyze-feature-requests` |

**Direktiften SONRA (artık read-only değil):**
| İhtiyaç | Kullan |
| :--- | :--- |
| Bulguları PRD'ye bağla | `pm-create-prd`, `dev-to-prd` → `.claude/rules/process.md` |
| Issue'ya dök | `dev-to-issues` |
| Mimari iyileştirme / refactor planı | `dev-improve-codebase-architecture`, `dev-request-refactor-plan` |

- Yeni proje (kullanıcıyı mülakatla): `dev-project-intake` (`/intake`).
