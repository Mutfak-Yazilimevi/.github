# Frontend Reference

> Kaynak: [#11 mikeoptimax/claude-frontend-skills-pack](https://github.com/mikeoptimax/claude-frontend-skills-pack) (MIT)
> Master plan kararı: skill **değil** — `rules/` altında **referans** olarak kullanılır.

Bu repo bir `CLAUDE.md` şablonu içerir (skill içermez). Kurulum sırasında upstream
`CLAUDE.md` içeriği buraya çekilir:

```bash
git clone --depth 1 https://github.com/mikeoptimax/claude-frontend-skills-pack ~/.skill-sources/frontend-pack
cp ~/.skill-sources/frontend-pack/CLAUDE.md .claude/rules/frontend-reference.md
```

> ⏳ **Doldurulacak:** Bu dosya şu an placeholder. Upstream içerik MIT lisanslı olup
> kurulum adımında kopyalanmalı. Lisans/atıf notunu koruyun.

## Frontend kuralları (özet)

İçerik çekilene kadar `code-style.md` içindeki "Frontend (varsa)" bölümü geçerlidir:
Prettier + ESLint, `PascalCase` component isimleri, component-temelli yapı.
