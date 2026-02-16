using Microsoft.EntityFrameworkCore;
using eTicaret.Models;

namespace eTicaret.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<Kampanya> Kampanyalar { get; set; }
        public DbSet<UrunVaryant> UrunVaryantlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>().ToTable("Kullanicilar");
            modelBuilder.Entity<Urun>().ToTable("Urunler");
            modelBuilder.Entity<Kategori>().ToTable("Kategoriler");
            modelBuilder.Entity<Siparis>().ToTable("Siparisler");
            modelBuilder.Entity<SiparisDetay>().ToTable("SiparisDetaylari");
            modelBuilder.Entity<Kampanya>().ToTable("Kampanyalar");
            modelBuilder.Entity<UrunVaryant>().ToTable("UrunVaryantlari");
        }
    }
}