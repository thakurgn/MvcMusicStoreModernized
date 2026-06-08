using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Models;

namespace MvcMusicStoreModernized.Data
{
    public class MusicStoreContext : IdentityDbContext<IdentityUser>
    {
        public MusicStoreContext(DbContextOptions<MusicStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Album>()
                .Property(a => a.Title)
                .HasMaxLength(160)
                .IsRequired();

            modelBuilder.Entity<Album>()
                .Property(a => a.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Album>()
                .Property(a => a.AlbumArtUrl)
                .HasMaxLength(1024);

            modelBuilder.Entity<Album>()
                .HasIndex(a => a.ArtistId);

            modelBuilder.Entity<Album>()
                .HasIndex(a => a.GenreId);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(a => a.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Genre)
                .WithMany(g => g.Albums)
                .HasForeignKey(a => a.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
    .HasKey(c => c.RecordId);

            modelBuilder.Entity<Cart>()
                .Property(c => c.CartId)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Album)
                .WithMany()
                .HasForeignKey(c => c.AlbumId);

            modelBuilder.Entity<Order>()
    .Property(o => o.Total)
    .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Album)
                .WithMany()
                .HasForeignKey(od => od.AlbumId);
        }
    }
}