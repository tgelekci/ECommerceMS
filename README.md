# ECommerceMS — Mikroservis Tabanlı E-Ticaret API'si

ASP.NET Core Web API ve .NET 9 kullanılarak geliştirilmiş mikroservis tabanlı bir e-ticaret backend projesidir.

## Servisler

| Servis | Port | Açıklama |
|---|---|---|
| ProductService | 5001 | Ürün yönetimi |
| OrderService | 5003 | Sipariş yönetimi |
| UserService | 5005 | Kullanıcı yönetimi |
| ApiGateway | 5000 | Tek giriş noktası |

## Kullanılan Teknolojiler

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 (InMemory)
- YARP (API Gateway)
- Mapster (Model dönüşümü)
- FluentValidation (Veri doğrulama)
- Swagger

## Çalıştırma

1. Solution'ı Visual Studio 2022'de aç
2. Tüm projeleri **Multiple Startup Projects** olarak ayarla
3. F5 ile çalıştır
4. Swagger arayüzlerine eriş:
   - `http://localhost:5001/swagger`
   - `http://localhost:5003/swagger`
   - `http://localhost:5005/swagger`
5. API Gateway üzerinden test et:
   - `http://localhost:5000/products`
   - `http://localhost:5000/orders`
   - `http://localhost:5000/users`
