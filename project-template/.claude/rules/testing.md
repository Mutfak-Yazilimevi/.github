# Testing

> Test yazım ve çalıştırma kuralları.

## İlkeler

- Her yeni feature için test yaz; davranışı test et, implementasyonu değil.
- AAA deseni: **Arrange — Act — Assert**.
- Testler bağımsız ve deterministik olmalı; paylaşılan durum yok.
- Bir test bir davranışı doğrular; isim `Metot_Senaryo_BeklenenSonuc` formatında.

## Katmanlara Göre

- **Domain:** saf birim testleri — bağımlılık yok, hızlı.
- **Application:** handler testleri — repository/dış servis mock'lanır.
- **Infrastructure:** entegrasyon testleri — gerçek DB (Testcontainers/PostgreSQL).
- **WebApi:** uçtan uca testler — `WebApplicationFactory` ile.

## Araçlar

- xUnit + FluentAssertions + NSubstitute (veya Moq).
- Entegrasyon için Testcontainers; coverage için coverlet.

## Komutlar

```bash
dotnet test                          # tüm testler
dotnet test --filter "Category=Unit" # yalnız birim testleri
```

## Kabul Kriteri

- CI'da testler yeşil olmadan PR merge edilmez.
- Kritik iş kuralları için coverage hedefi gözetilir.
