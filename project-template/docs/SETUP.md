# Kurulum — Kalan Kaynaklar

Çekirdek skill/agent kütüphanesi repoda **hazır gelir** (`.claude/skills/`, `.claude/agents/`).
Bazı kaynaklar ise bilinçli olarak repoya **commit edilmez** ve kurulumda çekilir.

## Neden bazı şeyler repoda yok?

| Kategori | Sebep |
| :--- | :--- |
| `_staging/` (lisanssız/koşullu) | Lisans YOK/belirsiz → org reposuna commit etmek hukuki risk; `.gitignore`'da |
| `_catalog/` (mega-kataloglar) | 1.500+ skill, çoğu yeniden paketlenmiş → şişkinlik; yalnız index amaçlı |
| `sec-*` (cybersec) | 754 skill; yalnız **savunma** alt kümesi, saldırı/exploit hariç → seçici çekim |

## Kullanım

```bash
# Koşullu/lisanssız kaynaklar (yalnız yerel inceleme; .claude/'a taşımadan önce lisans netliği bekle)
bash scripts/setup.sh --staging

# Mega-kataloglar (arama/index; import yok)
bash scripts/setup.sh --catalog

# Cybersec savunma alt kümesi → .claude/skills/sec-* (saldırgan içerik filtrelenir)
bash scripts/setup.sh --cybersec

# Hepsi
bash scripts/setup.sh --all
```

## Notlar

- `_staging/` ve `_catalog/` **auto-load EDİLMEZ** — Claude Code yalnız `.claude/skills/<ad>/SKILL.md`'i tarar.
- `_staging`'deki bir skill'i kullanmak istersen: lisansını netleştir → `.claude/skills/<önek-ad>/` altına taşı.
- `--cybersec` yalnız savunma/blue-team skill'lerini ekler; `exploiting-*`, `*-attack`, `pentest`, `metasploit`, `*-bypass` vb. desenler filtrelenir.
- `.NET` REST API + ASP.NET Core odaklı `.github/skills` çekirdeği için MCP gereksinimleri: `.mcp.json` (context7 + sequential-thinking).
