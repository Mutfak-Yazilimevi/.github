# `agent-memory/` — Sub-agent Kalıcı Hafızası

Sub-agent'ların oturumlar arası **kalıcı not/bağlam** sakladığı yer (2026). Ana konuşma
bağlamını şişirmeden, bir agent'ın öğrendiklerini (proje konvansiyonları, tekrar eden kararlar,
keşif notları) bir sonraki oturuma taşımak için kullanılır.

## Kullanım

- Her agent kendi dosyasına yazar: `agent-memory/<agent-adı>.md` (örn. `code-reviewer-pro.md`).
- İçerik **kısa ve damıtılmış** olmalı — ham log değil, kalıcı değer taşıyan özler.
- Hassas veri (secret, token, kişisel veri) **yazma**.

## Commit edilir mi?

- Takımla paylaşılacak, projeye özgü kalıcı bilgi → **commit et**.
- Kişisel/geçici notlar → `agent-memory/local/` altına koy ve gitignore'la (gerekirse).

> Boş başlamak normaldir; agent'lar gerektikçe doldurur. Bu klasör yalnız bir konvansiyon/
> yerleşim sağlar.
