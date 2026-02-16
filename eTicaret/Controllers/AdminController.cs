using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaret.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin kontrolü
        private bool AdminMi()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            return rol == "Admin";
        }

        // Admin ana sayfa
        public IActionResult Index()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.ToplamUrun = _context.Urunler.Count();
            ViewBag.ToplamKullanici = _context.Kullanicilar.Count();
            ViewBag.ToplamSiparis = _context.Siparisler.Count();

            // Düşük stoklu ürünler (10'dan az)
            ViewBag.DusukStokUrunler = _context.Urunler
                .Where(u => u.StokMiktari < 10)
                .ToList();

            return View();
        }

        // Ürün Listesi
        public async Task<IActionResult> Urunler()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .ToListAsync();

            return View(urunler);
        }

        // Ürün Ekleme Sayfası
        public async Task<IActionResult> UrunEkle()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View();
        }

        // Ürün Ekleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunEkle(Urun urun)
        {
            try
            {
                // Model validasyonu
                if (!ModelState.IsValid)
                {
                    ViewBag.Hata = "Lütfen tüm alanları doğru şekilde doldurun!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                // Ekstra fiyat kontrolü (double check)
                if (urun.Fiyat <= 0)
                {
                    ViewBag.Hata = "Fiyat 0'dan büyük olmalıdır!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                // Stok kontrolü
                if (urun.StokMiktari < 0)
                {
                    ViewBag.Hata = "Stok miktarı negatif olamaz!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                urun.EklenmeTarihi = DateTime.Now;
                _context.Urunler.Add(urun);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("Urunler");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Hata: Geçersiz veri girişi!";
                ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }
        }  // Ürün Düzenleme Sayfası
        public async Task<IActionResult> UrunDuzenle(int id)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        // Ürün Düzenleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunDuzenle(Urun urun)
        {
            try
            {
                // Model validasyonu
                if (!ModelState.IsValid)
                {
                    ViewBag.Hata = "Lütfen tüm alanları doğru şekilde doldurun!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                // Ekstra fiyat kontrolü
                if (urun.Fiyat <= 0)
                {
                    ViewBag.Hata = "Fiyat 0'dan büyük olmalıdır!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                // Stok kontrolü
                if (urun.StokMiktari < 0)
                {
                    ViewBag.Hata = "Stok miktarı negatif olamaz!";
                    ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                    return View(urun);
                }

                _context.Urunler.Update(urun);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction("Urunler");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Hata: Geçersiz veri girişi!";
                ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }
        }
        // Ürün Silme
        public async Task<IActionResult> UrunSil(int id)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun != null)
            {
                _context.Urunler.Remove(urun);
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Ürün silindi!";
            }

            return RedirectToAction("Urunler");
        }
        // Sipariş Listesi
        public async Task<IActionResult> Siparisler()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisVeren)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // Sipariş Durum Güncelle
        public async Task<IActionResult> SiparisDurumGuncelle(int id, string durum)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _context.Siparisler.FindAsync(id);

            if (siparis != null)
            {
                siparis.Durum = durum;
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Sipariş durumu güncellendi!";
            }

            return RedirectToAction("Siparisler");
        }
        // Varyant Yönetimi
        public async Task<IActionResult> Varyantlar(int id)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urun = await _context.Urunler
                .Include(u => u.Varyantlar)
                .FirstOrDefaultAsync(u => u.UrunID == id);

            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // Varyant Ekle
        [HttpPost]
        public async Task<IActionResult> VaryantEkle(int urunId, string renk, string beden, int stok)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var varyant = new UrunVaryant
                {
                    UrunID = urunId,
                    Renk = renk,
                    Beden = beden,
                    StokMiktari = stok
                };

                _context.UrunVaryantlari.Add(varyant);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Varyant eklendi!";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Varyantlar", new { id = urunId });
        }

        // Varyant Sil
        public async Task<IActionResult> VaryantSil(int id, int urunId)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var varyant = await _context.UrunVaryantlari.FindAsync(id);

            if (varyant != null)
            {
                _context.UrunVaryantlari.Remove(varyant);
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Varyant silindi!";
            }

            return RedirectToAction("Varyantlar", new { id = urunId });
        }
    }
}