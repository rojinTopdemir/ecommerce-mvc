namespace eTicaret.Models
{
    public class SepetItem
    {
        public int UrunID { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        public string? ResimYolu { get; set; }

        public decimal ToplamFiyat => Fiyat * Adet;
    }
}