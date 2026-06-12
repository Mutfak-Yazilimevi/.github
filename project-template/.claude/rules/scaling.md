# Scaling

> Ölçekleme kuralları. Tasarım eforunu ölçeğe göre ayarla — 1K kullanıcılık ürün için 10M'lik
> altyapı kurma (premature complexity), ama büyümeyi de tıkama. Aşama bazlı playbook ve tam
> matris için `dev-system-design-scaling` skill'ine devret.

## İlke

**Bugüne uyan en basit tasarım** + net evrim yolu. Daha üst aşamadan bir şey ödünç almadan önce
**ölçülmüş** bir darboğaz olmalı.

## Aşamalara göre varsayılanlar (özet)

| Boyut | 1K (MVP) | 1M (ölçekleniyor) | 10M+ (kurumsal) |
| :--- | :--- | :--- | :--- |
| Mimari | monolit / katmanlı | modüler monolit / erken servisler | dağıtık microservices |
| DB | tek relational DB | read replica + index | sharding / partition |
| Cache | yok denecek kadar | sıcak okumalar (Redis) | çok katmanlı (CDN/app/DB) |
| Async | senkron | yavaş işler kuyruğa | event-driven + streaming |
| Trafik | tek instance | load balancer + stateless | multi-region + CDN + autoscale |
| Gözlemlenebilirlik | temel log | log + metrik + alert | distributed tracing + SLO |
| Tutarlılık | güçlü (tek DB) | güçlü + güvenli yerlerde eventual | akış bazında bilinçli seçim + idempotency |

## Her aşamada geçerli kurallar

- Servisler **stateless** → yatay ölçeklenir; durumu veri katmanına/cache'e taşı.
- **Önce idempotency**, sonra retry/kuyruk/at-least-once.
- Cache eklemeden önce **invalidation** stratejisini planla.
- **Ölç, sonra ölçekle** — kapasite/karmaşıklığı gerçek darboğaza karşı ekle.
- **Hataya göre tasarla** — her uzak çağrı timeout olabilir; çök değil, zarif degrade et.
- Ölçekleme **kademeli** — servisi çıkar / tabloyu shard et yük dayatınca, önceden değil.

## Tipik evrim yolu

1. Tek DB → index → read replica → cache → write shard.
2. Inline iş → kuyruk → servisler arası event-driven entegrasyon.
3. Monolit → modüler monolit → en yüksek yüklü bounded context'i ilk çıkar.

> Bulut/maliyet için `cloud-architect`, performans için `performance-engineer`, DB için
> `database-optimizer` agent'larına devret.
