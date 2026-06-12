# Project: <ProjeAdı>

<Bir-iki cümle: proje ne yapar, mimari yaklaşım.>

> Skill & agent'lar merkezi marketplace'ten gelir (`mutfak-yazilimevi/claude-config`).
> Bu proje yalnız gerekli plugin'leri etkinleştirir — bkz. `.claude/settings.json` → `enabledPlugins`.
> Tüm kütüphaneyi projeye kopyalamaya gerek yok.

## Tech Stack
- Runtime: <.NET 10 / Node / …>
- DB: <PostgreSQL / …>
- Mimari: <Clean Architecture + Vertical Slice + CQRS / …>

## Commands
- <build / test / run komutları>

## Conventions
- <proje-özel kurallar>
- Etkin plugin setine göre skill'ler otomatik tetiklenir (progressive disclosure — düşük token).

## Etkin Plugin Profili
Bu proje için açık plugin'ler (`.claude/settings.json`):
- `mutfak-core`, `mutfak-dev`, `mutfak-dotnet`, `mutfak-security` _(örnek — projeye göre düzenle)_
