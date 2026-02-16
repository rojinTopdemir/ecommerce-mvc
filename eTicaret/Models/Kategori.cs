using System.ComponentModel.DataAnnotations;

namespace eTicaret.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriID { get; set; }
        public string KategoriAdi { get; set; }
        public string? Aciklama { get; set; }

        public List<Urun>? Urunler { get; set; }
    }
}