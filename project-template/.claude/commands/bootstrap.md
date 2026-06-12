---
description: Yeni bir feature (vertical slice) iskeleti oluştur
argument-hint: "<feature adı, örn. CreateOrder>"
allowed-tools: Read, Write, Edit, Bash(dotnet *), Grep, Glob
---

**$ARGUMENTS** feature'ı için bir vertical slice iskelesi kur.

1. Mevcut bir slice'ı örnek al (yapı/isim konvansiyonunu eşle) — `.claude/rules/architecture.md`.
2. Slice dosyalarını üret: Command/Query + Handler + Validator + Endpoint mapping (CQRS).
3. Domain'e dokunman gerekiyorsa entity/iş kuralını ekle; bağımlılık yönünü koru
   (WebApi → Infrastructure → Application → Domain).
4. İskelet birim testini ekle (`.claude/rules/testing.md`, AAA).
5. Endpoint'i host kompozisyonuna bağla; `dotnet build` ile doğrula.

Çıktı: oluşturulan dosyaların listesi + eksik kalan TODO'lar. İş mantığını uydurma —
belirsiz yerleri TODO bırak ve sor.
