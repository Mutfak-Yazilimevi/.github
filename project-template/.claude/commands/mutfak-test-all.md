---
description: Tüm test paketini çalıştır ve sonuçları özetle
argument-hint: "[opsiyonel: filtre, örn. Category=Unit]"
allowed-tools: Bash(dotnet *), Read
---

Tüm testleri çalıştır (filtre verildiyse: `$ARGUMENTS`).

1. `dotnet build` — derleme hatası varsa **dur** ve raporla.
2. `dotnet test` (filtre varsa `--filter "$ARGUMENTS"`).
3. Başarısız testleri grupla: hangi katman (Domain/Application/Infrastructure/WebApi),
   kök neden tahmini, ilgili dosya:satır.
4. Başarısızlık varsa düzeltmeyi öner; `/mutfak-fix-issue` veya `debugger` agent'ına yönlendir.

Çıktı: geçen/kalan sayısı + (varsa) kısa başarısızlık özeti. Hepsi yeşilse net belirt.
