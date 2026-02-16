using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class Kampanya
    {
        [Key]
        public int KampanyaID { get; set; }
        public string KampanyaAdi { get; set; }
        public decimal IndirimOrani { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public bool Aktif { get; set; } = true;
    }
} 