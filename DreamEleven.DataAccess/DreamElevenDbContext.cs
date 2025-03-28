using DreamEleven.Entities;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess
{
    public class DreamElevenDbContext : IdentityDbContext<User>
    {
        public DreamElevenDbContext(DbContextOptions<DreamElevenDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player enum conversion (PositionType)
            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasConversion<string>(); // enum → string

            // Comment tablosu ile Team tablosu arasında 1-N ilişki kur
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Team)              // Her yorum bir takıma aittir
                .WithMany(t => t.Comments)        // Her takımın birçok yorumu olabilir
                .HasForeignKey(c => c.TeamId);    // Foreign key: Comment.TeamId

            // TeamPlayer tablosu ile Team arasında 1-N ilişki kur
            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Team)            // Her TeamPlayer bir takıma aittir
                .WithMany(t => t.TeamPlayers)     // Her takımın birçok oyuncusu (TeamPlayer) olabilir
                .HasForeignKey(tp => tp.TeamId);  // Foreign key: TeamPlayer.TeamId

            // TeamPlayer tablosu ile Player arasında 1-N ilişki kur
            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Player)          // Her TeamPlayer bir oyuncuya bağlıdır
                .WithMany(p => p.TeamPlayers)     // Bir oyuncu birçok takımda olabilir (TeamPlayer)
                .HasForeignKey(tp => tp.PlayerId); // Foreign key: TeamPlayer.PlayerId

            modelBuilder.Entity<Player>()
                   .HasIndex(p => p.Slug)
                   .IsUnique();  // Slug'ı benzersiz yapıyoruz // futbolcu url si 

            // Diğer tablolarla ilgili başka konfigürasyonlar eklenebilir
        }
    }
}
