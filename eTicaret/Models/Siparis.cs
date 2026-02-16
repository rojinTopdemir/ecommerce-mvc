using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class Siparis
    {
        [Key]
        public int SiparisID { get; set; }
        public int KullaniciID { get; set; }
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = "Beklemede";
        public string? TeslimatAdresi { get; set; }

        public Kullanici? SiparisVeren { get; set; }
        public List<SiparisDetay>? SiparisDetaylari { get; set; }
    }
}