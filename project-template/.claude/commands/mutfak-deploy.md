---
description: Deploy öncesi kontrol listesini çalıştır ve yayın adımlarını hazırla
argument-hint: "[ortam: staging | production]"
allowed-tools: Bash, Read, Grep
---

Hedef ortam: **$ARGUMENTS** (verilmediyse `staging` varsay).

Deploy etmeden önce kontrol listesi:

1. **Yeşil mi?** `dotnet build` ve `dotnet test` — başarısızsa **dur** ve raporla.
2. **Migration:** Bekleyen EF Core migration var mı? (`dotnet ef migrations list`) —
   varsa uygulama planını belirt.
3. **Config/secrets:** Ortam değişkenleri ve secret'lar hedef ortam için hazır mı?
   Secret'ları asla loglama/echo'lama.
4. **Sağlık:** `/health` ve `/health/ready` endpoint'leri tanımlı mı?
5. **Sürüm:** Değişiklik özeti + (varsa) release notu taslağı.

`production` için: ek olarak geri-alma (rollback) planını ve blue-green/canary stratejisini
teyit et. Asıl deploy komutunu **kullanıcı onayı olmadan çalıştırma** — yalnız hazırla ve öner.
Altyapı/bulut kararları için `cloud-architect` veya `deployment-engineer` agent'ına devret.
