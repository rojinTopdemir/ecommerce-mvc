using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class UrunVaryant
    {
        [Key]
        public int VaryantID { get; set; }
        public int UrunID { get; set; }
        public string? Renk { get; set; }
        public string? Beden { get; set; }
        public int StokMiktari { get; set; }
        public decimal? EkFiyat { get; set; } // Varyanta göre ek fiyat

        // İlişki
        public Urun? Urun { get; set; }
    }
}