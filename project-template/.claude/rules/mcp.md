# MCP (Model Context Protocol)

> MCP sunucuları Claude'u dış sistemlere bağlar (GitHub, DB, tarayıcı, dokümantasyon).
> Konfig: proje kökündeki `.mcp.json` (takımla paylaşılır, git'e commit edilir). Sırlar
> dosyaya **yazılmaz** — `${ENV_VAR}` ile ortamdan gelir.

## Etkin sunucular (`.mcp.json`)

| Sunucu | Ne işe yarar | Gerekli env |
| :--- | :--- | :--- |
| `context7` | Güncel kütüphane/dokümantasyon çekme | — |
| `sequential-thinking` | Çok adımlı yapılandırılmış muhakeme | — |
| `github` | PR/issue/repo işlemleri | `GITHUB_PERSONAL_ACCESS_TOKEN` |
| `postgres` | Veritabanı sorgu/şema | `DATABASE_URL` |
| `playwright` | Tarayıcı otomasyonu / E2E / screenshot | — |

## Opsiyonel sunucular (ihtiyaç halinde `.mcp.json`'a ekle)

| Sunucu | Ne için | Not |
| :--- | :--- | :--- |
| Slack | Bildirim & arama | `SLACK_BOT_TOKEN` ister |
| Linear / JIRA | Ticket akışları | API token ister |
| Filesystem | Belirli dizinlere erişim | yol whitelist'i |

> Ekleme deseni (github örneğindeki gibi):
> ```json
> "slack": { "command": "npx", "args": ["-y", "<paket>"], "env": { "SLACK_BOT_TOKEN": "${SLACK_BOT_TOKEN}" } }
> ```

## Kurallar

- **Sır yok:** token/şifre/URL'ler `.mcp.json`'a düz yazılmaz; `${ENV_VAR}` kullan, env'i
  `.env`/secret yöneticisiyle sağla (`.env*` zaten gitignore + settings.json deny).
- **Minimum yetki:** yalnız gereken sunucuları etkinleştir (token + bağlam maliyeti).
- **Token gerektiren sunucu** token yoksa başlatılmayabilir — opsiyonelleri ihtiyaç anında ekle.
- Bildirim için `SLACK_WEBHOOK_URL` ayrıca `hooks/notify.sh` tarafından da kullanılır (MCP'den bağımsız).
