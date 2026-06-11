# Code Style

> .NET / C# odaklı kod stili kuralları. Claude bu kurallara uymalı.

## Genel

- `.editorconfig` kuralları bağlayıcıdır; commit öncesi `dotnet format` çalıştır.
- Anlamlı isimlendirme: değişken/metot/sınıf adları niyeti açıklamalı.
- Yorum yerine okunabilir kod tercih et; yorum "neden"i açıklar, "ne"yi değil.

## C# Konvansiyonları

- `PascalCase`: tip, metot, property, sabit · `camelCase`: yerel değişken, parametre.
- Private alanlar `_camelCase`.
- `var` yalnız tip sağ taraftan açıkça anlaşılıyorsa.
- Nullable reference types açık (`<Nullable>enable</Nullable>`).
- Async metotlar `Async` son ekiyle; `Task`/`ValueTask` döndür, `async void` kullanma.
- Erken `return` ile guard clause; derin iç içe `if` kaçın.
- LINQ okunabilirliği bozmadığı sürece tercih edilir.

## Dosya / Proje Düzeni

- Dosya başına tek public tip.
- Feature dosyaları kendi vertical slice klasöründe (Command/Query + Handler + Validator).
- `using` ifadeleri sadeleştirilmiş; global using'ler `GlobalUsings.cs` içinde.

## Frontend (varsa)

- Bkz. `frontend-reference.md` (referans CLAUDE.md, #11 frontend-pack).
- Prettier + ESLint; component isimleri `PascalCase`.
