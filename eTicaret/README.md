# 🛒 E-Ticaret Projesi

Modern ve profesyonel bir ASP.NET Core MVC 8.0 e-ticaret platformu.

## 🚀 Özellikler

### Kullanıcı Özellikleri
- ✅ Kullanıcı kayıt ve giriş sistemi
- ✅ Şifre hashleme (SHA256)
- ✅ Güçlü şifre doğrulama
- ✅ Oturum yönetimi
- ✅ Profil yönetimi

### Ürün Yönetimi
- ✅ Ürün listeleme ve detay görüntüleme
- ✅ Kategori bazlı filtreleme
- ✅ Renk ve beden varyantları
- ✅ Stok takibi
- ✅ Otomatik stok düşürme
- ✅ Düşük stok uyarıları

### Alışveriş
- ✅ Sepet sistemi
- ✅ Sipariş oluşturma
- ✅ Sipariş takibi
- ✅ Kampanya sistemi (%250 TL üzeri indirim)
- ✅ Otomatik kampanya hesaplama

### Admin Paneli
- ✅ Ürün ekleme, düzenleme, silme
- ✅ Varyant yönetimi
- ✅ Sipariş yönetimi
- ✅ Sipariş durum güncelleme
- ✅ Kampanya yönetimi
- ✅ Detaylı raporlama
  - Satış raporları
  - Stok raporları
  - Kampanya performans raporları
- ✅ Excel export

### Güvenlik
- ✅ SHA256 şifre hashleme
- ✅ CSRF token koruması
- ✅ SQL Injection koruması
- ✅ XSS koruması
- ✅ Form validasyonu

### Kullanıcı Deneyimi
- ✅ Modern ve responsive tasarım
- ✅ Toast bildirimleri
- ✅ Loading animasyonları
- ✅ 404/500 hata sayfaları
- ✅ Lazy loading

## 🛠️ Teknolojiler

- **Framework:** ASP.NET Core MVC 8.0
- **Veritabanı:** MySQL 8.0
- **ORM:** Entity Framework Core 8.0
- **Frontend:** Bootstrap 5.3, Font Awesome 6.4
- **Excel:** ClosedXML
- **Şifreleme:** SHA256

## 📋 Gereksinimler

- Visual Studio 2022 veya üzeri
- .NET 8.0 SDK
- MySQL 8.0 veya üzeri
- MySQL Workbench (önerilen)

## ⚙️ Kurulum

### 1. Veritabanı Kurulumu

MySQL Workbench'i açın ve şu sorguları sırayla çalıştırın:
```sql
-- Veritabanı oluştur
CREATE DATABASE EticaretDB;
USE EticaretDB;

-- Tablolar oluşturuluyor...
-- (Tüm tablo oluşturma scriptleri)
```

### 2. Bağlantı Ayarları

`appsettings.json` dosyasını açın ve MySQL bağlantı bilgilerinizi güncelleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=EticaretDB;user=root;password=SIFRENIZ"
  }
}
```

### 3. NuGet Paketlerini Yükle

Package Manager Console'da:
```
Update-Package
```

### 4. Projeyi Çalıştır

Visual Studio'da **F5** tuşuna basın veya:
```
dotnet run
```

## 👤 Test Kullanıcıları

### Admin Kullanıcı
- **Email:** admin@eticaret.com
- **Şifre:** admin123

### Normal Kullanıcı
Kayıt ol sayfasından yeni kullanıcı oluşturabilirsiniz.

## 📱 Özellik Detayları

### Kampanya Sistemi
- 250 TL ve üzeri alışverişlerde otomatik indirim
- Admin panelinden kampanya ekleme/düzenleme
- Kampanya aktif/pasif yapma
- Tarih bazlı kampanya kontrolü

### Raporlama
- **Günlük/Aylık/Yıllık** satış raporları
- **En çok satan ürünler** analizi
- **Stok durumu** raporları
- **Excel export** özelliği

### Stok Yönetimi
- Otomatik stok düşürme
- Düşük stok uyarıları (10'dan az)
- Stok sıfır olan ürünler için "Stokta Yok" gösterimi
- Varyant bazlı stok takibi

## 🎨 Tasarım Özellikleri

- Modern gradient arka planlar
- Smooth animasyonlar
- Hover efektleri
- Responsive tasarım (mobil uyumlu)
- Toast bildirimleri
- Loading spinner
- Profesyonel hata sayfaları

## 📂 Proje Yapısı
```
eTicaret/
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── HomeController.cs
│   ├── KampanyaController.cs
│   ├── RaporController.cs
│   ├── SepetController.cs
│   └── SiparisController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Kampanya.cs
│   ├── Kategori.cs
│   ├── Kullanici.cs
│   ├── PasswordHelper.cs
│   ├── SepetItem.cs
│   ├── Siparis.cs
│   ├── SiparisDetay.cs
│   ├── Urun.cs
│   └── UrunVaryant.cs
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Home/
│   ├── Kampanya/
│   ├── Rapor/
│   ├── Sepet/
│   ├── Siparis/
│   └── Shared/
└── wwwroot/
    └── images/
```

## 🔒 Güvenlik Notları

- Tüm şifreler SHA256 ile hashlenmiştir
- Form submit işlemlerinde CSRF token kullanılır
- SQL Injection'a karşı Entity Framework kullanılır
- XSS'e karşı Razor encoding kullanılır

## 📞 İletişim

Proje hakkında sorularınız için:
- Email: info@eticaret.com
- GitHub: [Proje Linki]

## 📝 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

**© 2025 E-Ticaret Projesi - Tüm hakları saklıdır.**