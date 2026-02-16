using eTicaret.Data;
using eTicaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace eTicaret.Controllers
{
    public class RaporController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RaporController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool AdminMi()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            return rol == "Admin";
        }

        // Satış Raporları Ana Sayfa
        public async Task<IActionResult> Index()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            // Bugünün satışları
            var bugun = DateTime.Today;
            var bugunSatislar = await _context.Siparisler
                .Where(s => s.SiparisTarihi.Date == bugun)
                .ToListAsync();

            // Bu ayın satışları
            var ayBaslangic = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ayinSatislari = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= ayBaslangic)
                .ToListAsync();

            // Bu yılın satışları
            var yilBaslangic = new DateTime(DateTime.Now.Year, 1, 1);
            var yilinSatislari = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= yilBaslangic)
                .ToListAsync();

            // Toplam satışlar
            var toplamSatislar = await _context.Siparisler.ToListAsync();

            ViewBag.BugunSatisSayisi = bugunSatislar.Count;
            ViewBag.BugunCiro = bugunSatislar.Sum(s => s.ToplamTutar);

            ViewBag.AylikSatisSayisi = ayinSatislari.Count;
            ViewBag.AylikCiro = ayinSatislari.Sum(s => s.ToplamTutar);

            ViewBag.YillikSatisSayisi = yilinSatislari.Count;
            ViewBag.YillikCiro = yilinSatislari.Sum(s => s.ToplamTutar);

            ViewBag.ToplamSatisSayisi = toplamSatislar.Count;
            ViewBag.ToplamCiro = toplamSatislar.Sum(s => s.ToplamTutar);

            // En çok satan ürünler
            var enCokSatanlar = await _context.SiparisDetaylari
                .Include(sd => sd.Urun)
                .GroupBy(sd => sd.UrunID)
                .Select(g => new
                {
                    UrunID = g.Key,
                    UrunAdi = g.First().Urun.UrunAdi,
                    ToplamSatis = g.Sum(sd => sd.Adet),
                    ToplamCiro = g.Sum(sd => sd.Adet * sd.BirimFiyat)
                })
                .OrderByDescending(x => x.ToplamSatis)
                .Take(5)
                .ToListAsync();

            ViewBag.EnCokSatanlar = enCokSatanlar;

            return View();
        }

        // Detaylı Satış Raporu
        public async Task<IActionResult> SatisRaporu(DateTime? baslangic, DateTime? bitis)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var baslangicTarih = baslangic ?? DateTime.Today.AddDays(-30);
            var bitisTarih = bitis ?? DateTime.Today;

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisVeren)
                .Include(s => s.SiparisDetaylari)
                .ThenInclude(sd => sd.Urun)
                .Where(s => s.SiparisTarihi >= baslangicTarih && s.SiparisTarihi <= bitisTarih)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            ViewBag.BaslangicTarih = baslangicTarih.ToString("yyyy-MM-dd");
            ViewBag.BitisTarih = bitisTarih.ToString("yyyy-MM-dd");
            ViewBag.ToplamCiro = siparisler.Sum(s => s.ToplamTutar);
            ViewBag.ToplamSiparis = siparisler.Count;

            return View(siparisler);
        }

        // Stok Raporu
        public async Task<IActionResult> StokRaporu()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .OrderBy(u => u.StokMiktari)
                .ToListAsync();

            ViewBag.DusukStok = urunler.Where(u => u.StokMiktari < 10).Count();
            ViewBag.TukenenStok = urunler.Where(u => u.StokMiktari == 0).Count();
            ViewBag.ToplamStokDegeri = urunler.Sum(u => u.Fiyat * u.StokMiktari);

            return View(urunler);
        }

        // Kampanya Performans Raporu
        public async Task<IActionResult> KampanyaRaporu()
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
        // Satış Raporu Excel Export
        public async Task<IActionResult> SatisRaporuExcel(DateTime? baslangic, DateTime? bitis)
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var baslangicTarih = baslangic ?? DateTime.Today.AddDays(-30);
            var bitisTarih = bitis ?? DateTime.Today;

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisVeren)
                .Include(s => s.SiparisDetaylari)
                .ThenInclude(sd => sd.Urun)
                .Where(s => s.SiparisTarihi >= baslangicTarih && s.SiparisTarihi <= bitisTarih)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Satış Raporu");

                // Başlık
                worksheet.Cell(1, 1).Value = "E-TİCARET SATIŞ RAPORU";
                worksheet.Range(1, 1, 1, 7).Merge().Style
                    .Font.SetBold().Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#6366f1"))
                    .Font.SetFontColor(XLColor.White);

                // Tarih bilgisi
                worksheet.Cell(2, 1).Value = $"Tarih Aralığı: {baslangicTarih:dd.MM.yyyy} - {bitisTarih:dd.MM.yyyy}";
                worksheet.Range(2, 1, 2, 7).Merge();

                // Boş satır
                worksheet.Cell(3, 1).Value = "";

                // Sütun başlıkları
                worksheet.Cell(4, 1).Value = "Sipariş No";
                worksheet.Cell(4, 2).Value = "Tarih";
                worksheet.Cell(4, 3).Value = "Müşteri";
                worksheet.Cell(4, 4).Value = "Email";
                worksheet.Cell(4, 5).Value = "Durum";
                worksheet.Cell(4, 6).Value = "Tutar";
                worksheet.Cell(4, 7).Value = "Ürün Sayısı";

                worksheet.Range(4, 1, 4, 7).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#e5e7eb"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Veri satırları
                int row = 5;
                foreach (var siparis in siparisler)
                {
                    worksheet.Cell(row, 1).Value = siparis.SiparisID;
                    worksheet.Cell(row, 2).Value = siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 3).Value = $"{siparis.SiparisVeren?.Ad} {siparis.SiparisVeren?.Soyad}";
                    worksheet.Cell(row, 4).Value = siparis.SiparisVeren?.Email;
                    worksheet.Cell(row, 5).Value = siparis.Durum;
                    worksheet.Cell(row, 6).Value = siparis.ToplamTutar;
                    worksheet.Cell(row, 7).Value = siparis.SiparisDetaylari?.Sum(sd => sd.Adet) ?? 0;
                    row++;
                }

                // Toplam satır
                worksheet.Cell(row, 1).Value = "TOPLAM";
                worksheet.Cell(row, 6).Value = siparisler.Sum(s => s.ToplamTutar);
                worksheet.Range(row, 1, row, 7).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#fef3c7"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                // Sütun genişlikleri
                worksheet.Column(1).Width = 12;
                worksheet.Column(2).Width = 18;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 12;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 12;

                // Para formatı
                worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00 ₺";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Satis_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
            }
        }

        // Stok Raporu Excel Export
        public async Task<IActionResult> StokRaporuExcel()
        {
            if (!AdminMi())
            {
                return RedirectToAction("Login", "Account");
            }

            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .OrderBy(u => u.StokMiktari)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stok Raporu");

                // Başlık
                worksheet.Cell(1, 1).Value = "STOK RAPORU";
                worksheet.Range(1, 1, 1, 6).Merge().Style
                    .Font.SetBold().Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#10b981"))
                    .Font.SetFontColor(XLColor.White);

                // Rapor tarihi
                worksheet.Cell(2, 1).Value = $"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}";
                worksheet.Range(2, 1, 2, 6).Merge();

                worksheet.Cell(3, 1).Value = "";

                // Sütun başlıkları
                worksheet.Cell(4, 1).Value = "Ürün ID";
                worksheet.Cell(4, 2).Value = "Ürün Adı";
                worksheet.Cell(4, 3).Value = "Kategori";
                worksheet.Cell(4, 4).Value = "Stok Miktarı";
                worksheet.Cell(4, 5).Value = "Birim Fiyat";
                worksheet.Cell(4, 6).Value = "Stok Değeri";

                worksheet.Range(4, 1, 4, 6).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#e5e7eb"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Veri satırları
                int row = 5;
                foreach (var urun in urunler)
                {
                    worksheet.Cell(row, 1).Value = urun.UrunID;
                    worksheet.Cell(row, 2).Value = urun.UrunAdi;
                    worksheet.Cell(row, 3).Value = urun.Kategori?.KategoriAdi ?? "-";
                    worksheet.Cell(row, 4).Value = urun.StokMiktari;
                    worksheet.Cell(row, 5).Value = urun.Fiyat;
                    worksheet.Cell(row, 6).Value = urun.StokMiktari * urun.Fiyat;

                    // Düşük stok uyarısı (sarı)
                    if (urun.StokMiktari < 10 && urun.StokMiktari > 0)
                    {
                        worksheet.Range(row, 1, row, 6).Style
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#fef3c7"));
                    }
                    // Tükenen stok (kırmızı)
                    else if (urun.StokMiktari == 0)
                    {
                        worksheet.Range(row, 1, row, 6).Style
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#fee2e2"));
                    }

                    row++;
                }

                // Toplam
                worksheet.Cell(row, 1).Value = "TOPLAM";
                worksheet.Cell(row, 6).Value = urunler.Sum(u => u.StokMiktari * u.Fiyat);
                worksheet.Range(row, 1, row, 6).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#d1fae5"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                // Sütun genişlikleri
                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 12;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;

                // Para formatı
                worksheet.Column(5).Style.NumberFormat.Format = "#,##0.00 ₺";
                worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00 ₺";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Stok_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
            }
        }
    }
}