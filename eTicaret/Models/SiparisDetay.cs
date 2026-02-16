using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class SiparisDetay
    {
        [Key]
        public int DetayID { get; set; }
        public int SiparisID { get; set; }
        public int UrunID { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }

        // İlişkiler
        public Siparis? Siparis { get; set; }
        public Urun? Urun { get; set; }
    }
}