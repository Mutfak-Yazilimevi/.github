# API Conventions

> REST API tasarım ve uygulama kuralları (ASP.NET Core Minimal APIs).

## Endpoint Tasarımı

- Kaynak temelli, çoğul isimler: `/api/orders`, `/api/orders/{id}`.
- HTTP metotları: `GET` (oku), `POST` (oluştur), `PUT` (tam güncelle), `PATCH` (kısmi), `DELETE` (sil).
- Versiyonlama URL'de: `/api/v1/...`.
- Filtreleme/sayfalama query string ile: `?page=1&pageSize=20&sort=createdAt`.

## İstek / Yanıt

- Request/response için ayrı DTO'lar; domain entity'leri sızdırma.
- Doğrulama FluentValidation ile; başarısızlıkta `400` + `ProblemDetails`.
- Tutarlı hata formatı: RFC 7807 `ProblemDetails`.

## Status Kodları

| Durum | Kod |
| :--- | :--- |
| Başarılı okuma | `200 OK` |
| Oluşturma | `201 Created` + `Location` header |
| İçeriksiz başarı | `204 No Content` |
| Doğrulama hatası | `400 Bad Request` |
| Yetkisiz / yasak | `401` / `403` |
| Bulunamadı | `404 Not Found` |
| Çakışma | `409 Conflict` |
| Sunucu hatası | `500` |

## Vertical Slice

- Her endpoint kendi slice'ında: Command/Query + Handler + Validator + Endpoint mapping.
- Endpoint'ler ince; iş mantığı handler'da (CQRS).

## Dokümantasyon & Sağlık

- OpenAPI: Scalar ile yayımlanır.
- `/health` ve `/health/ready` health check endpoint'leri.
- Audit alanları (`CreatedAt`, `UpdatedAt`) EF Core interceptor ile otomatik.
