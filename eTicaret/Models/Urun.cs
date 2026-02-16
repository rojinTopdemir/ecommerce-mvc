using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class Urun
    {
        [Key]
        public int UrunID { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
        public string UrunAdi { get; set; }

        [StringLength(1000)]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0.01 ile 999999.99 arasında olmalıdır")]
        public decimal Fiyat { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0 veya pozitif olmalıdır")]
        public int StokMiktari { get; set; }

        public int? KategoriID { get; set; }

        [StringLength(500)]
        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        public string? ResimYolu { get; set; }

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public Kategori? Kategori { get; set; }
        public List<UrunVaryant>? Varyantlar { get; set; }
    }
}