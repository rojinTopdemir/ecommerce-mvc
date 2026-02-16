using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace eTicaret.Controllers
{
    public class SiparisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiparisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sipariş Onay Sayfası
        public IActionResult Onayla()
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var sepet = SepetGetir();

            if (sepet == null || sepet.Count == 0)
            {
                TempData["Hata"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Sepet");
            }

            return View(sepet);
        }

        // Sipariş Oluştur
        [HttpPost]
        public async Task<IActionResult> Olustur(string teslimatAdresi)
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);

            if (kullanici == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı!";
                return RedirectToAction("Login", "Account");
            }

            var sepet = SepetGetir();

            if (sepet == null || sepet.Count == 0)
            {
                TempData["Hata"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Sepet");
            }

            try
            {
                // Sipariş oluştur
                var siparis = new Siparis
                {
                    KullaniciID = kullanici.KullaniciID,
                    SiparisTarihi = DateTime.Now,
                    ToplamTutar = sepet.Sum(s => s.ToplamFiyat),
                    Durum = "Beklemede",
                    TeslimatAdresi = teslimatAdresi
                };

                _context.Siparisler.Add(siparis);
                await _context.SaveChangesAsync();

                // Sipariş detaylarını ekle
                foreach (var item in sepet)
                {
                    var detay = new SiparisDetay
                    {
                        SiparisID = siparis.SiparisID,
                        UrunID = item.UrunID,
                        Adet = item.Adet,
                        BirimFiyat = item.Fiyat
                    };

                    _context.SiparisDetaylari.Add(detay);

                    // Stoktan düş
                    var urun = await _context.Urunler.FindAsync(item.UrunID);
                    if (urun != null)
                    {
                        urun.StokMiktari -= item.Adet;
                    }
                }

                await _context.SaveChangesAsync();

                // Sepeti temizle
                HttpContext.Session.Remove("Sepet");

                TempData["Basarili"] = "Siparişiniz başarıyla oluşturuldu!";
                return RedirectToAction("Detay", new { id = siparis.SiparisID });
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Sipariş oluşturulurken hata: " + ex.Message;
                return RedirectToAction("Onayla");
            }
        }

        // Sipariş Detay
        public async Task<IActionResult> Detay(int id)
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisDetaylari)
                .ThenInclude(sd => sd.Urun)
                .FirstOrDefaultAsync(s => s.SiparisID == id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        // Siparişlerim
        public async Task<IActionResult> Siparislerim()
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciID == kullanici.KullaniciID)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // Helper
        private List<SepetItem> SepetGetir()
        {
            var sepetJson = HttpContext.Session.GetString("Sepet");

            if (string.IsNullOrEmpty(sepetJson))
            {
                return new List<SepetItem>();
            }

            return JsonSerializer.Deserialize<List<SepetItem>>(sepetJson) ?? new List<SepetItem>();
        }
    }
}