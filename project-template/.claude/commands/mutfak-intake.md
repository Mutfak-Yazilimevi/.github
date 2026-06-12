---
description: Yeni proje ön kapısı — rehberli brief al, düşün, fikir sun, PRD'ye bağla
argument-hint: "[varsa: proje hakkında bir-iki cümle]"
allowed-tools: Read, Write, Grep, Glob
---

Yeni bir projeye başlıyoruz. `dev-project-intake` skill'ini kullanarak **rehberli ön kapıyı** yürüt.

Başlangıç girdisi (varsa): **$ARGUMENTS**

1. 7 alanlık brief'i topla (`docs/intake-template.md`): ne/kim · problem · olmazsa-olmaz ·
   kapsam dışı · kısıtlar · başarı ölçütü · referans. Hepsini birden dayatma.
2. Eksikleri **tek tek, somut seçeneklerle** sorarak çıkar; sadece yazıya dökme — sonuç,
   risk ve atladığım seçenekleri de sun, bir öneri ver, kapsamı yönlendir.
3. MVP / sonra / kapsam-dışı sınırını netleştir (en az 2 kapsam-dışı madde).
4. Anlaşılan brief'i **kabul kriterli PRD**'ye bağla (`pm-create-prd`/`dev-to-prd`):
   user story + Given/When/Then + NFR + başarı metriği.
5. PRD'yi ben onaylamadan tasarım/uygulamaya geçme — kapı budur. Sonra `rules/process.md`.

Bilinmeyenleri uydurma; açık soru olarak listele.

> Komşu ihtiyaçlar (skill'in "ne zaman hangisi" tablosuna bak): strateji/iş brief'i → `ali-brief`
> (`/cs:brief`) · ürün fırsatı doğrulama → `ali-product-discovery` · ürün vizyonu → `pm-product-vision`.
