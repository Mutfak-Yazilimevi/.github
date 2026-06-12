---
description: Mevcut proje ön kapısı (SALT-OKUNUR) — anla, yapılan/eksikleri çıkar, backlog'a yaz, planı açıkla, direktif bekle
argument-hint: "[opsiyonel: odak alan/dizin]"
allowed-tools: Read, Grep, Glob, Bash(find:*), Bash(ls:*), Bash(pwd), Bash(wc:*), Bash(git log:*), Bash(git status:*)
---

Bu mevcut bir proje. `dev-existing-project-onboarding` skill'ini kullan. Odak (varsa): **$ARGUMENTS**

⚠️ **SALT-OKUNUR SÖZLEŞME:** Kaynağa/config'e/şemaya **hiçbir değişiklik yapma**. Düzeltme,
refactor, format, kurulum, migration, git-yazma **yok**. Sorularımın cevaplarını **kod içinde** ara.
İzin verilen tek yazma: `docs/onboarding-report.md` ve `docs/backlog.md`.

1. **Anla:** README, CLAUDE.md, docs, manifestler, giriş noktaları, klasör yapısı, testler →
   amaç, stack, mimari, feature'lar, entegrasyonlar, nasıl çalıştığı. Büyük repoda `Explore` ile yay.
2. **Yapılan vs Eksik:** iki liste çıkar (kanıt: `dosya:satır`). TODO/FIXME, test boşlukları,
   yarım/bozuk alanlar, eski bağımlılık, güvenlik/mimari kokuları → eksiklere.
3. **Backlog:** fikir/fırsatları `docs/backlog.md`'ye öncelikli maddeler olarak yaz (yalnız fikir,
   değişiklik değil). Bir şey bozuksa düzeltme — backlog'a kaydet.
4. **Planı açıkla:** ne olduğu + yapılan/eksik özeti + yazdığın backlog + önerilen iş sırası
   (gerekçe + risk) + açık sorular.
5. **DUR ve direktif bekle:** PRD üretme, işe başlama, kod değiştirme. Bitişte sor:
   "Backlog hazır + plan açıklandı. PRD'ye bağlamam için hangi maddeleri seçiyorsunuz?"

Yalnızca açık direktifle seçilen maddeleri PRD'ye bağla (`pm-create-prd`) ve `rules/process.md`'ye geç.

> İlişkili (skill'in "ne zaman hangisi" tablosu): anlama → `ali-codebase-onboarding`, `ali-code-tour`,
> `ali-monorepo-navigator` · boşluk → `pm-intended-vs-implemented`, `ali-tech-debt-tracker`,
> `ali-dependency-auditor` · direktif sonrası → `dev-improve-codebase-architecture`, `dev-to-issues`.
