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

            // 🟡 Enum string olarak saklansın
            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasConversion<string>();

            // 🔵 Slug benzersiz olsun (SEO-friendly URL için)
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // 🟢 Team → TeamPlayers (1 - N)
            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Team)             // Her TeamPlayer bir takıma aittir
                .WithMany(t => t.TeamPlayers)      // Her takımın birçok oyuncusu (TeamPlayer) olabilir
                .HasForeignKey(tp => tp.TeamId)    // Foreign key: TeamPlayer.TeamId
                .OnDelete(DeleteBehavior.Cascade); // Takım silinirse oyuncuları da silinsin

            // 🟢 TeamPlayer → Player (1 - N)
            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Player)           // Her TeamPlayer bir oyuncuya bağlıdır
                .WithMany(p => p.TeamPlayers)      // Bir oyuncu birçok takımda olabilir (TeamPlayer)
                .HasForeignKey(tp => tp.PlayerId)  // Foreign key: TeamPlayer.PlayerId
                .OnDelete(DeleteBehavior.Cascade);

            // 🟢 Comment → Team (1 - N)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Team)               // Her yorum bir takıma aittir
                .WithMany(t => t.Comments)         // Her takımın birçok yorumu olabilir
                .HasForeignKey(c => c.TeamId)      // Foreign key: Comment.TeamId
                .OnDelete(DeleteBehavior.Cascade);

            // 🔴 Navigation olmayan Foreign Key alanlar için zorunlu işaretleme
            modelBuilder.Entity<Team>()
                .Property(t => t.UserId)
                .IsRequired();                     // UserId boş geçilemez

            modelBuilder.Entity<Comment>()
                .Property(c => c.UserId)
                .IsRequired();                     // UserId boş geçilemez
        }
    }
}
