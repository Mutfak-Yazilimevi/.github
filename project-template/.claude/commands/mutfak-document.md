---
description: Kod/araç için dokümantasyon üret (API, README, ADR)
argument-hint: "[hedef: endpoint/feature/dosya veya 'api']"
allowed-tools: Read, Write, Edit, Grep, Glob, Bash(git *)
---

Hedef için dokümantasyon üret: **$ARGUMENTS** (verilmezse son değişiklikleri belgele).

1. Kapsamı belirle: yeni/değişen endpoint, feature veya modül.
2. Uygun çıktı:
   - **API** → OpenAPI/Scalar uyumlu açıklamalar, istek/yanıt örnekleri (RFC 7807 hatalar).
   - **README/kullanım** → ne, neden, nasıl; minimal ve güncel.
   - **Mimari karar** → `docs/adr/0000-template.md`'den yeni ADR.
3. Mevcut dokümana uy; tekrar/çelişki üretme. Derin iş için `documentation-expert` veya
   `api-documenter` agent'ına devret.

Çıktı: güncellenen/oluşturulan doküman dosyaları. Var olmayan davranışı belgeleme.
