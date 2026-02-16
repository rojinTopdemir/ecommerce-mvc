using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eTicaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.Varyantlar)  // ← Bunu ekle!
                .OrderByDescending(u => u.EklenmeTarihi)
                .Take(12)
                .ToListAsync();

            return View(urunler);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // 404 ve diğer hata sayfaları
        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            ViewBag.StatusCode = statusCode;

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Aradığınız sayfa bulunamadı!";
                    ViewBag.ErrorIcon = "fa-search";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Sunucuda bir hata oluştu!";
                    ViewBag.ErrorIcon = "fa-server";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Bu sayfaya erişim yetkiniz yok!";
                    ViewBag.ErrorIcon = "fa-lock";
                    break;
                default:
                    ViewBag.ErrorMessage = "Bir hata oluştu!";
                    ViewBag.ErrorIcon = "fa-exclamation-triangle";
                    break;
            }

            return View("Error");
        }
    }

}