# 💄 Beauty Center Management System

Güzellik merkezleri için geliştirilmiş kapsamlı yönetim sistemi. Multi-tenant mimarisi ile birden fazla güzellik merkezini tek bir platform üzerinden yönetmenize olanak sağlar.

## 🌟 Özellikler

### 👥 Kullanıcı Yönetimi
- **Multi-Tenant Yapısı**: Birden fazla güzellik merkezi desteği
- **Rol Bazlı Erişim**: SuperAdmin, TenantAdmin, Employee rolleri
- **Güvenli Kimlik Doğrulama**: JWT token tabanlı authentication
- **Kullanıcı İzinleri**: Rol bazında özellik erişim kontrolü

### 📅 Randevu Yönetimi
- **Randevu Oluşturma**: Müşteri ve hizmet bazlı randevu sistemi
- **Randevu Takibi**: Randevu durumlarının gerçek zamanlı takibi
- **Takvim Görünümü**: Haftalık ve aylık randevu görünümleri
- **Otomatik Bildirimler**: Randevu hatırlatmaları

### 👤 Müşteri Yönetimi
- **Müşteri Profilleri**: Detaylı müşteri bilgi sistemi
- **Müşteri Geçmişi**: Randevu ve ödeme geçmişi takibi
- **İletişim Bilgileri**: Telefon, email ve adres yönetimi

### 🛍️ Hizmet Yönetimi
- **Hizmet Tanımları**: Çeşitli güzellik hizmetleri tanımlama
- **Fiyat Yönetimi**: Hizmet başına fiyatlandırma sistemi
- **Hizmet Kategorileri**: Kategorize edilmiş hizmet yapısı

### 💰 Ödeme Yönetimi
- **Taksitli Ödeme**: Esnek ödeme planları
- **Ödeme Takibi**: Ödeme durumlarının detaylı takibi
- **Çoklu Ödeme Yöntemleri**: Nakit, kredi kartı, havale desteği
- **Finansal Raporlar**: Detaylı gelir-gider raporları

### 📊 Raporlama Sistemi
- **Finansal Raporlar**: Gelir, gider ve kar-zarar analizi
- **Müşteri Raporları**: Müşteri analiz ve istatistikleri
- **Randevu Raporları**: Randevu yoğunluk analizi
- **Excel Export**: Raporları Excel formatında dışa aktarma

### 🔧 Süper Admin Paneli
- **Tenant Yönetimi**: Güzellik merkezlerini yönetme
- **Kullanıcı Oluşturma**: Yeni kullanıcı ve admin hesapları oluşturma
- **Sistem İstatistikleri**: Genel sistem performans metrikleri
- **Yetkilendirme**: Kullanıcı rol ve izin yönetimi

## 🛠️ Teknoloji Stack

### Backend (.NET 8 Web API)
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server + Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt
- **Architecture Pattern**: Repository Pattern + Generic Repository
- **API Documentation**: Swagger/OpenAPI
- **Dependency Injection**: Built-in ASP.NET Core DI

### Frontend (Blazor Server)
- **Framework**: Blazor Server (.NET 9)
- **UI Framework**: Bootstrap 5
- **Icons**: Font Awesome
- **State Management**: Built-in Blazor state management
- **HTTP Client**: HttpClient with custom ApiService
- **Authentication**: Custom AuthStateProvider
- **Local Storage**: Blazored.LocalStorage

### Database
- **RDBMS**: SQL Server
- **ORM**: Entity Framework Core
- **Migrations**: Code-First approach
- **Data Seeding**: Automatic demo data generation

### DevOps & Tools
- **Version Control**: Git
- **IDE**: Visual Studio / VS Code
- **Package Manager**: NuGet
- **Build System**: MSBuild

## 🏗️ Sistem Mimarisi

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Blazor Server  │────│   Web API       │────│   SQL Server    │
│   Frontend      │    │   Backend       │    │   Database      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
        │                       │                       │
        │                       │                       │
    ┌───▼───┐              ┌────▼────┐             ┌────▼────┐
    │  UI   │              │ Business│             │  Data   │
    │ Layer │              │  Logic  │             │ Layer   │
    └───────┘              └─────────┘             └─────────┘
```

### Multi-Tenant Architecture
- **Tenant Isolation**: Her güzellik merkezi kendi verilerine erişir
- **Shared Database**: Tek veritabanı, tenant ID ile veri izolasyonu
- **Middleware**: TenantMiddleware ile otomatik tenant tespiti
- **Security**: Tenant bazlı veri güvenliği



## 🔑 Demo Hesapları

### SuperAdmin
- **Kullanıcı Adı**: `superadmin`
- **Şifre**: `admin123`

### Tenant Admin Hesapları
- **Gülhan Güzellik Merkezi**: `gulhan` / `admin123`
- **Arda Güzellik Merkezi**: `arda` / `admin123`
- **Elif Güzellik Merkezi**: `elif` / `admin123`
- **Demo Güzellik Merkezi**: `demo` / `admin123`

## 🗂️ Proje Yapısı

```
BeautyCenterFrontend/
├── BeautyCenterApi/                 # Backend Web API
│   ├── Controllers/                 # API Controllers
│   ├── Models/                      # Entity Models
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Services/                    # Business Logic Services
│   ├── Repositories/                # Data Access Layer
│   ├── Middleware/                  # Custom Middleware
│   ├── Data/                        # Entity Framework Context
│   └── Migrations/                  # Database Migrations
│
├── BeautyCenterFrontend/            # Frontend Blazor App
│   ├── Components/                  # Blazor Components
│   │   ├── Pages/                   # Page Components
│   │   └── Layout/                  # Layout Components
│   ├── Models/                      # Frontend Models
│   ├── Services/                    # Frontend Services
│   └── wwwroot/                     # Static Files
│
└── README.md                        # Proje Dokümantasyonu
```

## 🔒 Güvenlik Özellikleri

- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: BCrypt ile güvenli şifre hashleme
- **Role-Based Authorization**: Rol bazlı erişim kontrolü
- **Tenant Isolation**: Multi-tenant veri güvenliği
- **HTTPS Support**: Güvenli iletişim protokolü
- **Input Validation**: Kapsamlı veri doğrulama

## 📊 Performans

- **Optimize Edilmiş Sorgular**: Entity Framework optimize edilmiş sorgular
- **Lazy Loading**: Gerektiğinde veri yükleme
- **Caching**: Memory cache ile performans iyileştirmesi
- **Pagination**: Büyük veri setleri için sayfalama
- **Async Operations**: Asenkron işlemler ile yüksek performans

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'i push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakın.

## 📞 İletişim

**Proje Maintainer**: Tülay Çetin
**Email**: [email@example.com]
**GitHub**: [@tulaycetin](https://github.com/tulaycetin)

## 🙏 Teşekkürler

Bu projeyi geliştirirken kullanılan teknolojiler ve açık kaynak kütüphanelerin geliştiricilerine teşekkürler.

---

⭐ Bu projeyi beğendiyseniz star vermeyi unutmayın!
