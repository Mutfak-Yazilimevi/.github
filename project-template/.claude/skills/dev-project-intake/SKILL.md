---
name: project-intake
description: Guided front door for STARTING a new project. Interview the user with a structured brief (problem, users, must-haves, out-of-scope, constraints, success criteria), think along, propose ideas and trade-offs, steer scope, then hand the finished brief off to a PRD with acceptance criteria. Use at project kickoff when requirements are vague and need to be elicited, shaped, and turned into a spec before design or build.
---

# Project Intake (new project front door)

## Overview

A new project rarely arrives as a clean spec. This skill is the **front door**: take a rough
idea, run a guided interview, think along, propose ideas and trade-offs, steer scope, and
produce a **filled brief → PRD with acceptance criteria**. The user should not have to write a
perfect spec — you elicit it.

Be an active partner, not a form. Ask, suggest, challenge, then converge.

## Workflow

1. **Capture the rough brief.** Invite the 7 fields (don't demand all at once):
   *ne/kim · problem · olmazsa-olmaz (MVP) · kapsam dışı · kısıtlar · başarı ölçütü · referans.*
   Template: `docs/intake-template.md`. Pair with `dev-idea-refine` / `dev-define-goal`.
2. **Interview the gaps.** Ask targeted, one-thread-at-a-time questions for anything missing or
   ambiguous. Prefer concrete choices over open prompts. Use `dev-interview-me` and the
   `spec-analyst` agent for thorough elicitation.
3. **Think & propose.** Don't just transcribe — surface implications, risks, and options the
   user didn't mention (auth? data model? scale? compliance? edge cases?). Recommend a default,
   explain the trade-off, let them decide. Steer away from scope creep.
4. **Converge on scope.** Lock MVP vs later vs out-of-scope. Force at least 2 explicit
   out-of-scope items — the boundary is as important as the goal.
5. **Bind to a PRD.** Turn the agreed brief into a PRD: user stories + **acceptance criteria**
   (Given/When/Then), non-functional requirements, success metrics. Use `pm-create-prd` /
   `dev-to-prd`. Validate with `pm-validate-idea` / `pm-prioritize-features`.
6. **Gate.** The user approves the PRD before design/implementation begins. Then hand off to
   `spec-architect` / `rules/process.md` lifecycle.

## Interview lenses (what to probe)

- **Users & jobs** — who, what job-to-be-done, primary vs secondary.
- **Must-haves vs nice-to-haves** — force a ranking; MVP is the smallest lovable slice.
- **Data** — core entities, sources, volume, sensitivity (PII?).
- **Integrations** — external systems, auth, third-party APIs.
- **Non-functional** — scale (see `dev-system-design-scaling`), latency, availability, security.
- **Constraints** — stack/platform preference, deadline, budget, regulation.
- **Success** — "done looks like…" → seeds acceptance criteria.

## Output

A short **project brief** (filled template) + a **PRD draft** (stories + acceptance criteria +
NFRs + success metrics), explicitly approved before build. Unknowns listed as open questions,
not silently assumed.

## Gotchas

- **Order-taking** — transcribing without challenging scope or surfacing risks; be a partner.
- **Boil-the-ocean MVP** — everything is "must-have"; force ranking and out-of-scope.
- **Inventing requirements** — fill gaps by asking, not assuming; list unknowns explicitly.
- **Skipping the gate** — building before the user signs off the PRD.

## Related — ne zaman hangisi

Bu skill, **yazılım projesi gereksinim ön kapısıdır** (brief → kabul kriterli PRD). Komşu
brief/discovery skill'leri farklı amaçlara hizmet eder — doğru olanı seç:

| İhtiyaç | Kullan |
| :--- | :--- |
| Yazılım projesi gereksinimi → PRD (bu skill) | **`dev-project-intake`** (`/mutfak-intake`) |
| Mevcut projeyi salt-okunur anla → backlog | `dev-existing-project-onboarding` (`/mutfak-onboard`) |
| Tek-sayfa **strateji/iş brief'i** (office-hours, danışmanlık) | `ali-brief` (`/cs:brief`) |
| Ürün **fırsatı doğrulama / discovery sprint** | `ali-product-discovery` |
| Ürün **vizyonu** / fikir doğrulama | `pm-product-vision`, `pm-validate-idea` |
| Fikri netleştir / beyin fırtınası | `dev-idea-refine`, `dev-define-goal`, `dev-brainstorming` |
| Brief → PRD'ye dönüştür | `pm-create-prd`, `dev-to-prd` · agent `spec-analyst` |

Akış: (gerekirse strateji/discovery için yukarıdakiler) → **`dev-project-intake`** → PRD →
`dev-development-lifecycle` / `.claude/rules/process.md`. Derin mülakat: `dev-interview-me`.
