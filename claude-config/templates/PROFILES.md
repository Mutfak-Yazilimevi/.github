# Proje Tipi → Plugin Profilleri

Yeni bir projede `.claude/settings.json` → `enabledPlugins` için başlangıç önerileri.
**İlke:** sadece ilgili plugin'leri aç → en az bağlam/token, ortalama-üstü performans.

| Proje tipi | Önerilen plugin'ler |
| :--- | :--- |
| **.NET REST API** | `mutfak-core`, `mutfak-dev`, `mutfak-dotnet`, `mutfak-security` |
| **Web frontend (React/Next)** | `mutfak-core`, `mutfak-dev`, `mutfak-frontend`, `mutfak-design` |
| **Full-stack SaaS** | `mutfak-core`, `mutfak-dev`, `mutfak-dotnet`, `mutfak-frontend`, `mutfak-design`, `mutfak-security` |
| **Mobil (RN/Flutter/Native)** | `mutfak-core`, `mutfak-dev`, `mutfak-frontend` |
| **Pazarlama / büyüme / içerik sitesi** | `mutfak-core`, `mutfak-marketing`, `mutfak-pm`, `mutfak-design` |
| **Ürün keşfi / strateji** | `mutfak-core`, `mutfak-pm`, `mutfak-consulting` |
| **Spec-driven büyük proje** | yukarıdakilere ek `mutfak-spec-workflow` |
| **Güvenlik/SOC odaklı** | `mutfak-core`, `mutfak-dev`, `mutfak-security` |
| **Araştırma/akademik** | `mutfak-core`, `mutfak-research`, `mutfak-diagrams` |

> Not: Her plugin yüzlerce skill içerse de, skill'ler "progressive disclosure" ile çalışır
> (yalnız ad+açıklama yüklenir; tetiklenince gövde gelir) — bu yüzden çok plugin açmak yerine
> **yalnız gerekenleri** açmak token'ı minimumda tutar.
