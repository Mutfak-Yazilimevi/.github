# `hooks/` — Hook'lar (deterministik guardrail/otomasyon)

Hook'lar, Claude'un kararına bağlı olmayan **deterministik** script'lerdir; belirli olaylarda
çalışır. `settings.json` → `hooks` altında olay → matcher → komut olarak bağlanır.
Çıkış kodu: `0` izin/ok · `2` engelle (stderr Claude'a döner).

## Olay taksonomisi

| Olay | Ne zaman | Bu template'te |
| :--- | :--- | :--- |
| `PreToolUse` | Araç çalışmadan önce (engelleyebilir) | `validate-bash.sh` — tehlikeli Bash desenlerini engeller |
| `PostToolUse` | Araç çalıştıktan sonra | `format-code.sh` — Write/Edit sonrası best-effort format |
| `SessionStart` | Oturum açılışında (bağlam yükler) | `session-start.sh` — kısa proje bağlamı |
| `SessionEnd` | Oturum kapanışında | `session-end.sh` — agent-memory damıtma hatırlatıcısı |
| `PreCompact` | Context sıkıştırmadan önce | `secret-scan.sh` — sır tespit "gate" (uyarır) |
| `Notification` | Claude bildirim ürettiğinde | `notify.sh` — opsiyonel Slack/webhook |
| `Stop` | Ana yanıt tamamlandığında | `notify.sh` — opsiyonel bildirim |
| `SubagentStop` | Bir sub-agent bittiğinde | `notify.sh` — opsiyonel bildirim |

## İlkeler

- **Asla akışı kırma:** araç/koşul yoksa `exit 0` ile sessiz çık (format/notify/secret-scan böyle).
- **Hızlı tut:** `timeout` ver; ağ çağrıları opt-in (env ile) olsun.
- **Hassas veri yazma:** log/memory'ye secret koyma; `secret-scan.sh` tam da bunu yakalar.
- `notify.sh` yalnız `SLACK_WEBHOOK_URL` set ise gönderir (varsayılan: sessiz).

## Yeni hook ekleme

1. `hooks/<ad>.sh` yaz (`chmod +x`), stdin'den hook JSON'unu oku.
2. `settings.json` → `hooks.<Olay>` altına `matcher` + `command` ekle.
3. Engelleme gerekiyorsa `exit 2` + stderr'e gerekçe yaz.
