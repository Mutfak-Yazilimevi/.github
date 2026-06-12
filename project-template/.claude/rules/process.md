# Process — Geliştirme Yaşam Döngüsü (SDLC / ADLC)

> Bu proje işi **tanımlı bir süreç** olarak yürütür — rastgele uzman çağırma değil. Aşağıdaki
> faz haritası "hangi fazda hangi skill/agent/komut" sorusunu yanıtlar. Derin rehber ve karar
> ölçütleri için `dev-development-lifecycle` skill'ine devret.

## SDLC mi, ADLC mi?

- **SDLC** (insan-sürücülü) — her faz insan kapısıdır; agent yardımcıdır. Yüksek risk,
  regülasyon veya belirsizlik varsa varsayılan.
- **ADLC** (agent-sürücülü) — agent'lar fazları yürütür; insan **eval/onay** ile yönlendirir.
  İyi tanımlı, tekrarlanabilir iş için varsayılan. PRD ve skill'ler net olmalı.

İkisi aynı omurgayı paylaşır; ADLC'de planlama/PRD/skill'ler dinamik evrilir ve alt-agent'lar
fazları paralel koşar.

## Faz → ne kullanılır (özet)

| Faz (SDLC ↔ ADLC) | Skill / Agent / Komut |
| :--- | :--- |
| Planlama / Goal | `dev-planning-and-task-breakdown`, `dev-define-goal` · agent `spec-planner` |
| Gereksinim / PRD | `pm-create-prd`, `dev-to-prd` · agent `spec-analyst` |
| Tasarım | `dev-dotnet-architecture-selection`, `dev-system-design-scaling` · agent `spec-architect` · `rules/architecture.md` |
| (ADLC) Write Skills | `dev-skill-creator`, `dev-write-a-skill` + **`rules/capability-gaps.md`** |
| Orkestrasyon | **`mutfak-spec-workflow`**, `tech-lead-orchestrator`, `agent-organizer` |
| Uygulama | `dev-executing-plans`, `dev-tdd`, `dev-low-level-design` · agent `spec-developer` · **`/bootstrap`** |
| Test & QA | `dev-test-driven-development` · agent `spec-tester`, `qa-expert` · **`/test-all`** |
| Eval & Review | `dev-verification-before-completion` · agent `spec-reviewer`, `spec-validator` · **`/review`** |
| Deploy | `dev-ci-cd-and-automation` · agent `deployment-engineer` · **`/deploy`** |
| Bakım | `dev-deprecation-and-migration` · agent `legacy-modernizer`, `debugger` · **`/fix-issue`** |
| İzleme & Geri besleme | `dev-observability-and-instrumentation` · agent `performance-engineer` |

## Hazır omurga: spec-workflow

Önemsiz olmayan feature'ları tüm döngüyü kapsayan agent zinciriyle sür:

```
spec-analyst → spec-architect → spec-planner → spec-developer
            → spec-tester → spec-reviewer → spec-validator     (spec-orchestrator koordine eder)
```

## Kurallar

- **Süreci işe ölçekle** — tek satırlık düzeltme için tüm fazları koşturma.
- **Kapıları atlama** — kırmızı test veya onaysız PRD ile ilerleme; rework aşağı akar.
- **Önce skill** — agent orkestrasyonundan önce gerekli skill yoksa `capability-gaps` ile üret.
- **Döngüyü kapat** — İzleme/Eval bulguları yeni iş olarak Planlama/Goal'a geri girer.
- Karar/ölçüt detayları: `dev-development-lifecycle` skill'i.
