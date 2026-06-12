---
description: Mevcut diff/PR'ı kalite, güvenlik ve mimari uyum açısından incele
argument-hint: "[opsiyonel: dosya/dizin veya PR no]"
allowed-tools: Bash(git *), Read, Grep, Glob
---

Çalışma ağacındaki değişiklikleri (veya belirtilen hedefi: `$ARGUMENTS`) incele.

1. Kapsamı çıkar: `git diff --stat` ve `git diff` (hedef verildiyse ona daralt).
2. `.claude/rules/` kurallarına uyumu denetle — `code-style.md`, `testing.md`,
   `api-conventions.md`, `architecture.md`.
3. Şunlara odaklan: doğruluk hataları, güvenlik açıkları, mimari ihlaller
   (Clean/VSA/CQRS), test boşlukları, gereksiz karmaşıklık.
4. Derin inceleme gerekiyorsa `code-reviewer-pro` agent'ına devret; güvenlik ağırlıklıysa
   `security-auditor` agent'ını kullan.

Çıktı: önem derecesine göre (kritik/önemli/öneri) gruplanmış, dosya:satır referanslı,
eyleme dönük bulgular. Temizse bunu açıkça belirt.
