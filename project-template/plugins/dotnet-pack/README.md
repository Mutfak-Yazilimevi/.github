# dotnet-pack

.NET odaklı olgun küme (master plan Faz 7). Kaynak: #16 dotnet/skills + SA4 Aaronontheweb.

## Üye skill'ler (`dev-dotnet-*`, #16 — test/diag/ai plugin'leri)
`.claude/skills/dev-dotnet-*` — `scripts/build-plugins.sh` ile `./skills/` altına materyalize edilir.

## Üye agent'lar (SA4, MIT)
- akka-net-specialist
- roslyn-incremental-generator-specialist
- dotnet-concurrency-specialist
- dotnet-performance-analyst
- dotnet-benchmark-designer
- docfx-specialist

## Materyalize / dağıtım
```bash
bash scripts/build-plugins.sh dotnet-pack
```
Skill/agent içeriği `.claude/`'tan kopyalanır (repo'da çift saklanmaz; build anında üretilir).
