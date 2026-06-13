# Proje Tipi → Plugin Profilleri

Yeni bir projede `.claude/settings.json` → `enabledPlugins` için başlangıç önerileri.
**İlke:** sadece ilgili plugin'leri aç → en az bağlam/token, ortalama-üstü performans.

| Proje tipi | Önerilen plugin'ler |
| :--- | :--- |
| **.NET REST API** | `mutfak-core`, `mutfak-dev`, `mutfak-dotnet`, `mutfak-security-defense` |
| **Web frontend (React/Next)** | `mutfak-core`, `mutfak-dev`, `mutfak-frontend`, `mutfak-design` |
| **Full-stack SaaS** | `mutfak-core`, `mutfak-dev`, `mutfak-dotnet`, `mutfak-frontend`, `mutfak-design`, `mutfak-security-defense` |
| **Mobil (RN/Flutter/Native)** | `mutfak-core`, `mutfak-dev`, `mutfak-frontend` |
| **Pazarlama / büyüme / içerik sitesi** | `mutfak-core`, `mutfak-marketing`, `mutfak-pm`, `mutfak-design` |
| **Ürün keşfi / strateji** | `mutfak-core`, `mutfak-pm`, `mutfak-consulting` |
| **Spec-driven büyük proje** | yukarıdakilere ek `mutfak-spec-workflow` |
| **Mavi takım / SOC** | `mutfak-core`, `mutfak-security-detection`, `mutfak-security-forensics`, `mutfak-security-intel`, `mutfak-security-grc` |
| **Kırmızı takım / pentest** | `mutfak-core`, `mutfak-dev`, `mutfak-security-offensive` |
| **Güvenlik mühendisliği / hardening** | `mutfak-core`, `mutfak-dev`, `mutfak-security-defense`, `mutfak-security-grc` |
| **Araştırma/akademik** | `mutfak-core`, `mutfak-research`, `mutfak-diagrams` |

> **Güvenlik 6 alt-plugin'e bölündü** (token tasarrufu): `-offensive` · `-detection` ·
> `-forensics` · `-defense` · `-grc` · `-intel`. Tüm güvenlik alanlarına ihtiyaç nadirdir;
> yalnız rolüne uyan alt-plugin(ler)i aç.

> Not: Her plugin yüzlerce skill içerse de, skill'ler "progressive disclosure" ile çalışır
> (yalnız ad+açıklama yüklenir; tetiklenince gövde gelir) — bu yüzden çok plugin açmak yerine
> **yalnız gerekenleri** açmak token'ı minimumda tutar.
