using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaret.Controllers
{
    public class KampanyaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KampanyaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool AdminMi()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            return rol == "Admin";
        }

        // Kampanya Listesi (Admin)
        public async Task<IActionResult> Index()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var kampanyalar = await _context.Kampanyalar
                .OrderByDescending(k => k.BaslangicTarihi)
                .ToListAsync();

            return View(kampanyalar);
        }

        // Kampanya Ekle
        public IActionResult Ekle()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(Kampanya kampanya)
        {
            try
            {
                _context.Kampanyalar.Add(kampanya);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Kampanya eklendi!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Hata: " + ex.Message;
                return View(kampanya);
            }
        }

        // Kampanya Aktif/Pasif
        public async Task<IActionResult> AktifPasif(int id)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var kampanya = await _context.Kampanyalar.FindAsync(id);

            if (kampanya != null)
            {
                kampanya.Aktif = !kampanya.Aktif;
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Kampanya durumu güncellendi!";
            }

            return RedirectToAction("Index");
        }

        // Kampanya İndirimini Hesapla
        public decimal KampanyaIndirimiHesapla(decimal toplamTutar)
        {
            var aktifKampanya = _context.Kampanyalar
                .Where(k => k.Aktif && k.BaslangicTarihi <= DateTime.Now && k.BitisTarihi >= DateTime.Now)
                .FirstOrDefault();

            if (aktifKampanya != null)
            {
                return toplamTutar * (aktifKampanya.IndirimOrani / 100);
            }

            return 0;
        }
    }
}