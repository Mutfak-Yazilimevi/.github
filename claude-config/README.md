# Mutfak Yazılımevi — Claude Code Merkezi Konfigürasyon (Plugin Marketplace)

Bu repo, organizasyonun **tüm Claude Code skill & agent kütüphanesini** tek yerde tutan bir
**plugin marketplace**'idir. Projeler kütüphaneyi kopyalamaz; yalnız **gerekli plugin'leri
etkinleştirir** → en az token, ortalama-üstü performans.

> **Durum:** Bu içerik şu an `mutfak-yazilimevi/.github/claude-config/` altında hazırlandı.
> Kalıcı yeri ayrı bir **`mutfak-yazilimevi/claude-config`** reposudur (aşağıdaki "Taşıma" adımları).

## Mimari

```
claude-config (bu repo) ── marketplace.json
  plugins/
    mutfak-core · mutfak-dev · mutfak-dotnet · mutfak-frontend · mutfak-design
    mutfak-pm · mutfak-marketing · mutfak-security · mutfak-research
    mutfak-diagrams · mutfak-consulting · mutfak-spec-workflow
        ▲ referans (kopya değil)
   proje-A/.claude/settings.json   →  enabledPlugins: [dotnet, dev, security]
   proje-B/.claude/settings.json   →  enabledPlugins: [frontend, design]
```

12 plugin, **1606 skill + 64 agent** kapsar (önek bazlı paketlenmiş).

## Plugin'ler

| Plugin | İçerik (yakl.) |
| :--- | :--- |
| `mutfak-core` | genel/çekirdek skill'ler + genel agent'lar (~16 skill) |
| `mutfak-dev` | genel geliştirme + dil/mimari/data/devops agent'ları (~145 skill, 30 agent) |
| `mutfak-dotnet` | .NET skill'leri + backend-architect/Akka.NET/Roslyn/perf agent'ları (37 skill, 7 agent) |
| `mutfak-frontend` | premium-ui/uiux/generative + React/Next/UI/UX agent'ları (8 skill, 7 agent) |
| `mutfak-design` | 99 vendor design sistemi |
| `mutfak-pm` | ürün yönetimi (78 skill) |
| `mutfak-marketing` | pazarlama/SEO/growth + blog agent'ları (103 skill, 5 agent) |
| `mutfak-security` | savunma/blue-team güvenlik (754 skill) + security-auditor |
| `mutfak-research` | akademik/derin araştırma (4 skill) |
| `mutfak-diagrams` | markdown diyagram/görselleştirme (15 skill) |
| `mutfak-consulting` | iş/ops/C-level danışmanlık (347 skill) |
| `mutfak-spec-workflow` | spec-driven orkestrasyon (12 agent) |

## Kurulum (proje tarafı)

Bir projede marketplace'i ekleyip plugin etkinleştirme:

```bash
# Claude Code içinde
/plugin marketplace add mutfak-yazilimevi/claude-config
/plugin install mutfak-dotnet@mutfak
/plugin install mutfak-security@mutfak
```

…veya projeye `.claude/settings.json` koyup `enabledPlugins` tanımla (bkz.
`templates/project/.claude/settings.json` ve `templates/PROFILES.md`).

## Marketplace'i Doldurma (build)

Manifest'ler repoda; **skill/agent içeriği build aninda materyalize edilir**:

```bash
bash scripts/build-marketplace.sh --source <Mutfak skill kütüphanesi .claude dizini>
# örn: --source ../project-template/.claude
```

Bu, skill'leri önek bazlı (`dev-`, `sec-`, `mkt-`, `design-`, `ali-`, `md-`, …) ve agent'ları
eşleşmeye göre `plugins/<ad>/{skills,agents}/` altına yerleştirir.

## Yayımlama (tek komut) — `publish.sh`

`claude-config` reposunu kapsamına alan bir Claude Code oturumunda, repo kökünde:

```bash
bash scripts/publish.sh
```

Bu; `.github`'tan scaffold + skill kütüphanesini çeker, 12 plugin'i materyalize eder,
commit + push eder. (Kütüphane güncellenince tekrar çalıştırılabilir.)

> İlk kez boş repodaysan script henüz yok demektir; şu tek satırla bootstrap et:
> ```bash
> git clone --depth 1 https://github.com/Mutfak-Yazilimevi/.github /tmp/gh && bash /tmp/gh/claude-config/scripts/publish.sh
> ```

## Bu içeriği gerçek `claude-config` reposuna taşıma (manuel)

`.github` reposunun şişmemesi için materyalize içerik burada `.gitignore`'da. Yayımlamak için:

```bash
# 1) Organizasyon yöneticisi: GitHub'da boş 'mutfak-yazilimevi/claude-config' reposu oluştur
# 2) Bu klasörü yeni repoya taşı:
cp -r claude-config/* /yeni/claude-config/ && cd /yeni/claude-config
# 3) Plugin içeriğini materyalize et:
bash scripts/build-marketplace.sh --source <kütüphane/.claude>
# 4) Marketplace reposunda içerik commit edilmeli — .gitignore'daki
#    plugins/*/skills ve agents satırlarını kaldır VEYA:
git add -f plugins/ && git add -A
git commit -m "Mutfak plugin marketplace" && git push
```

## Proje Başına Akış (otomatik)

1. **Sen:** projeyi netleştirirsin ("X: .NET REST API + PostgreSQL").
2. **Ben (org erişimi + oturum kapsamı verildiğinde):** repoyu açar/klonlarım.
3. **Ben:** proje tipine göre `templates/PROFILES.md`'den plugin profilini seçerim.
4. **Ben:** minimal `.claude/` üretirim — `settings.json` (marketplace + `enabledPlugins`) +
   proje-özel `CLAUDE.md` + gerekirse `.mcp.json`. **Kütüphane kopyalanmaz, referanslanır.**
5. Proje açıldığında Claude Code plugin'leri marketplace'ten çeker.

## Yetenek Boşluğu Döngüsü (kütüphane büyür)

Bir projede **karşılığı olmayan** (mevcut skill/agent kapsamayan) bir konu çıktığında akış:

1. **Tespit:** Görev mevcut hiçbir skill/agent'ın net kapsamadığı bir alan içeriyor.
2. **Öner:** Tek seferlik ad-hoc çözmek yerine yeni **skill** (prosedür/bilgi) veya **agent**
   (otonom uzman persona) üretmeyi öner — kural: `project-template/.claude/rules/capability-gaps.md`.
3. **Üret:** Doğru önekle (`dev-`, `sec-`, `mkt-`, …) skill veya frontmatter'lı agent yaz;
   yardımcı meta-skill'ler: `dev-skill-creator`, `ali-agent-designer`, `dev-plugin-creator`.
4. **Merkeze ekle:** İçeriği `.github/project-template/.claude/` altına koy (tek kaynak).
5. **Yayımla:** `bash scripts/publish.sh` → marketplace tazelenir, **tüm projeler** faydalanır.

## Neden bu yaklaşım?
- **Kopya yok:** her proje sadece küçük bir `settings.json` taşır.
- **Performans:** progressive disclosure + yalnız ilgili plugin → minimum token.
- **Tek kaynak:** kütüphane bir kez burada güncellenir, tüm projeler faydalanır.
