---
description: Davranışı değiştirmeden kod yapısını/okunabilirliğini iyileştir
argument-hint: "<dosya/dizin veya açıklama>"
allowed-tools: Read, Edit, Grep, Glob, Bash(dotnet *)
---

Şu hedefi refactor et: **$ARGUMENTS**

1. **Önce yeşil:** mevcut testler geçiyor mu? Geçmiyorsa refactor'a başlama.
2. Kod kokularını tespit et: uzun metot/sınıf, tekrar, kötü isim, derin iç içe, primitive
   obsession, feature envy — `.claude/rules/code-style.md`, `dev-low-level-design`.
3. **Küçük, davranış-koruyan** adımlar; her adımdan sonra testleri çalıştır (`dotnet test`).
4. Davranış değişikliği **yapma** — gerekiyorsa ayır ve `/fix-issue` olarak öner.
5. Büyük/yapısal refactor için `code-refactorer-agent` veya `dev-improve-codebase-architecture`.

Çıktı: yapılan değişiklikler + neden (kok→çözüm) + testlerin yeşil kaldığı teyidi.
