using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaret.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kayıt sayfası
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Kullanici kullanici)
        {
            try
            {
                // Şifre güçlü mü kontrol et
                if (!PasswordHelper.IsStrongPassword(kullanici.Sifre))
                {
                    ViewBag.Hata = "Şifre en az 6 karakter olmalı ve büyük harf, küçük harf ve rakam içermelidir!";
                    return View(kullanici);
                }

                // Email formatı kontrolü
                if (!IsValidEmail(kullanici.Email))
                {
                    ViewBag.Hata = "Geçerli bir email adresi giriniz!";
                    return View(kullanici);
                }

                // Email kontrolü
                var mevcutKullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == kullanici.Email);

                if (mevcutKullanici != null)
                {
                    ViewBag.Hata = "Bu email adresi zaten kayıtlı!";
                    return View(kullanici);
                }

                // Şifreyi hashle
                kullanici.Sifre = PasswordHelper.HashPassword(kullanici.Sifre);
                kullanici.Rol = "Musteri";
                kullanici.KayitTarihi = DateTime.Now;

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();

                // Session'a kaydet
                HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
                HttpContext.Session.SetString("KullaniciAd", kullanici.Ad);
                HttpContext.Session.SetString("KullaniciRol", kullanici.Rol);
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                TempData["Basarili"] = "Kayıt başarılı! Hoş geldiniz.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return View(kullanici);
            }
        }

        // Email format kontrolü
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Giriş sayfası
        public IActionResult Login()
        {
            return View();
        }

        // Giriş işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            try
            {
                // Boş alan kontrolü
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre))
                {
                    ViewBag.Hata = "Email ve şifre alanları boş bırakılamaz!";
                    return View();
                }

                // Kullanıcıyı email ile bul
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == email);

                // Kullanıcı var mı ve şifre doğru mu kontrol et
                if (kullanici != null && PasswordHelper.VerifyPassword(sifre, kullanici.Sifre))
                {
                    // Session'a kaydet
                    HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
                    HttpContext.Session.SetString("KullaniciAd", kullanici.Ad);
                    HttpContext.Session.SetString("KullaniciRol", kullanici.Rol);
                    HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                    TempData["Basarili"] = "Giriş başarılı! Hoş geldiniz.";

                    // Admin ise admin paneline yönlendir
                    if (kullanici.Rol == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Hata = "Email veya şifre yanlış!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }

        // Çıkış
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}