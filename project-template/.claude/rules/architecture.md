# Architecture

> Mimari karar kuralları. Bu proje **Clean Architecture + Vertical Slice + CQRS** kullanır;
> bu doküman tercihi ve alternatifleri kayda geçirir. Derin tasarım için `dotnet-backend-architect`
> agent'ına, mimari seçim/karşılaştırma için `dev-dotnet-architecture-selection` skill'ine devret.

## Bu projenin mimarisi

```
WebApi → Infrastructure → Application → Domain
```

- **Clean** — bağımlılık yönü içe doğru; Domain hiçbir katmana bağımlı değil.
- **Vertical Slice** — her feature kendi slice'ında uçtan uca (Command/Query + Handler + Validator + Endpoint).
- **CQRS** — yazma (command) ve okuma (query) modelleri ayrı.

## Ne zaman hangi mimari? (özet)

| Stil | En uygun |
| :--- | :--- |
| Monolithic | küçük uygulama, MVP, prototip, basit domain |
| N-Tier | net katman ayrımı isteyen kurumsal uygulama |
| **Clean** | karmaşık, uzun ömürlü, test edilebilir kalması gereken proje |
| **Vertical Slice** | modern API, feature-team hızı, düşük cross-feature kuplaj |
| Microservices | çok takımlı, bağımsız ölçek/deploy gerektiren büyük sistem |
| **CQRS** | yüksek okuma/yazma asimetrisi, raporlama, karmaşık domain |
| Event-Driven | gerçek zamanlı, gevşek kuplaj, servisler arası entegrasyon, audit/replay |

> Stiller birleştirilebilir; "tek doğru mimari" yoktur. **En basit uyan** stili seç, gerektikçe
> birleştir — erkenden aşırı mimari kurma (premature complexity). Tam pro/kon ve seçim kontrol
> listesi: `dev-dotnet-architecture-selection` skill'i.

## Kurallar

- Domain saf kalır: framework/persistence/UI sızdırma.
- Bağımlılıklar abstraction'a; yön daima içe doğru (Dependency Inversion).
- Her feature tek slice; iş mantığı handler'da, endpoint ince.
- Mimari değişikliği gerektiren kararları `docs/adr/` altında ADR olarak kaydet.
- Yeni bir mimari desen/teknoloji boşluğu çıkarsa: önce mevcut skill/agent'a bak, yoksa
  merkezî kütüphaneye skill öner (bkz. `capability-gaps.md`).
