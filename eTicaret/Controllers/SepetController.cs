using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace eTicaret.Controllers
{
    public class SepetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SepetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sepeti Görüntüle
        public IActionResult Index()
        {
            var sepet = SepetGetir();

            // Kampanya kontrolü
            var toplamTutar = sepet.Sum(s => s.ToplamFiyat);
            var aktifKampanya = _context.Kampanyalar
                .Where(k => k.Aktif && k.BaslangicTarihi <= DateTime.Now && k.BitisTarihi >= DateTime.Now)
                .FirstOrDefault();

            ViewBag.ToplamTutar = toplamTutar;
            ViewBag.KampanyaIndirim = 0m;
            ViewBag.GenelToplam = toplamTutar;

            if (aktifKampanya != null && toplamTutar >= 250)
            {
                ViewBag.KampanyaIndirim = toplamTutar * (aktifKampanya.IndirimOrani / 100);
                ViewBag.GenelToplam = toplamTutar - ViewBag.KampanyaIndirim;
                ViewBag.KampanyaAdi = aktifKampanya.KampanyaAdi;
                ViewBag.KampanyaOran = aktifKampanya.IndirimOrani;
            }

            return View(sepet);
        }
        // Sepete Ürün Ekle
        public async Task<IActionResult> Ekle(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);

            if (urun == null || urun.StokMiktari < 1)
            {
                TempData["Hata"] = "Ürün bulunamadı veya stokta yok!";
                return RedirectToAction("Index", "Home");
            }

            var sepet = SepetGetir();
            var mevcutItem = sepet.FirstOrDefault(s => s.UrunID == id);

            if (mevcutItem != null)
            {
                mevcutItem.Adet++;
            }
            else
            {
                sepet.Add(new SepetItem
                {
                    UrunID = urun.UrunID,
                    UrunAdi = urun.UrunAdi,
                    Fiyat = urun.Fiyat,
                    Adet = 1,
                    ResimYolu = urun.ResimYolu
                });
            }

            SepetKaydet(sepet);
            TempData["Basarili"] = urun.UrunAdi + " sepete eklendi!";
            return RedirectToAction("Index", "Home");
        }

        // Sepetten Ürün Çıkar
        public IActionResult Cikar(int id)
        {
            var sepet = SepetGetir();
            var item = sepet.FirstOrDefault(s => s.UrunID == id);

            if (item != null)
            {
                sepet.Remove(item);
                SepetKaydet(sepet);
                TempData["Basarili"] = "Ürün sepetten çıkarıldı!";
            }

            return RedirectToAction("Index");
        }

        // Adet Güncelle
        public IActionResult AdetGuncelle(int id, int adet)
        {
            if (adet < 1)
            {
                return RedirectToAction("Cikar", new { id });
            }

            var sepet = SepetGetir();
            var item = sepet.FirstOrDefault(s => s.UrunID == id);

            if (item != null)
            {
                item.Adet = adet;
                SepetKaydet(sepet);
            }

            return RedirectToAction("Index");
        }

        // Sepeti Temizle
        public IActionResult Temizle()
        {
            HttpContext.Session.Remove("Sepet");
            TempData["Basarili"] = "Sepet temizlendi!";
            return RedirectToAction("Index");
        }

        // Helper Metodlar
        private List<SepetItem> SepetGetir()
        {
            var sepetJson = HttpContext.Session.GetString("Sepet");

            if (string.IsNullOrEmpty(sepetJson))
            {
                return new List<SepetItem>();
            }

            return JsonSerializer.Deserialize<List<SepetItem>>(sepetJson) ?? new List<SepetItem>();
        }

        private void SepetKaydet(List<SepetItem> sepet)
        {
            var sepetJson = JsonSerializer.Serialize(sepet);
            HttpContext.Session.SetString("Sepet", sepetJson);
        }
    }
}