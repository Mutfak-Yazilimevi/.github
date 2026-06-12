---
description: Bir issue/hatayı sistematik olarak teşhis edip düzelt (TDD ile)
argument-hint: "<issue no, hata mesajı veya açıklama>"
allowed-tools: Bash, Read, Edit, Write, Grep, Glob
---

Şu sorunu çöz: **$ARGUMENTS**

1. **Üret/anla:** Sorunu en küçük tekrar üreten senaryoya indir. Belirsizse `debugger`
   agent'ına ya da `dev-systematic-debugging` skill'ine devret.
2. **Önce test:** Hatayı yakalayan başarısız bir test yaz (kırmızı) — `dev-tdd` /
   `.claude/rules/testing.md`.
3. **Düzelt:** Minimal, hedeflenmiş değişiklik. Mimari kuralları koru (Clean/VSA/CQRS).
4. **Doğrula:** `dotnet test` (veya proje test komutu) yeşil; regresyon yok.
5. **Özetle:** Kök neden + yapılan değişiklik + eklenen test. Commit mesajı öner
   (kullanıcı istemeden commit/push etme).
