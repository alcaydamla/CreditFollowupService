# CreditFollowupService

CreditFollowupService, gecikmiş kredi ödemelerini Oracle veritabanından çekerek ilgili bilgileri otomatik e-posta yoluyla bildiren .NET Core tabanlı bir batch uygulamasıdır.

## Özellikler

- Oracle veritabanına bağlantı (CRMUSER kullanıcısı ile)
- `GET_OVERDUE_CUSTOMERS` adlı stored procedure üzerinden veri çekme
- 90+ gün gecikmiş ödemeleri tespit etme
- Gecikmiş müşteri listesini e-posta olarak iletme
- Tek seferlik çalışıp otomatik kapanma (batch mantığı)

## Teknolojiler

- .NET Core / C#
- Oracle Managed Data Access
- BackgroundService (Microsoft.Extensions.Hosting)
- SMTP üzerinden e-posta gönderimi

## E-posta Örneği

```text
- Ahmet Yılmaz: 514 gün gecikmiş, borç: ₺5.000,00
