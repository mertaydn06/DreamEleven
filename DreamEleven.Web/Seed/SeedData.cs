using DreamEleven.DataAccess;
using DreamEleven.Entities;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.Web
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            DreamElevenDbContext context)
        {

            if (context.Database.GetPendingMigrations().Any()) // Eğer herhangi bir migration bekliyorsa
            {
                await context.Database.MigrateAsync(); // Migration işlemini uygula
            }


            // Admin ve User rolleri yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));


            // Admin kullanıcı
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "Admin123*";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true // Eğer e-posta doğrulaması zorunlu değilse
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Normal kullanıcı
            const string userEmail = "user@gmail.com";
            const string userPassword = "User123*";

            var appUser = await userManager.FindByEmailAsync(userEmail);

            if (appUser == null)
            {
                appUser = new User
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(appUser, userPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appUser, "User");
                }
            }

            // Oyuncular
            if (!context.Players.Any())
            {
                var players = new List<Player>
                {
                    new Player { Name = "Lionel Messi", Overall = 93, RealTeam = "Inter Miami", Position = Player.PositionType.Forward, ImageUrl = "https://img.a.transfermarkt.technology/portrait/big/28003-1740766555.jpg?lm=1", Slug = "lionel-messi" },
                    new Player { Name = "Cristiano Ronaldo", Overall = 91, RealTeam = "Al-Nassr", Position = Player.PositionType.Forward, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/8198-1694609670.jpg?lm=1", Slug = "cristiano-ronaldo" },
                    new Player { Name = "Kevin De Bruyne", Overall = 91, RealTeam = "Man City", Position = Player.PositionType.Midfielder, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/88755-1713391485.jpg?lm=1", Slug = "kevin-de-bruyne" },
                    new Player { Name = "Erling Haaland", Overall = 91, RealTeam = "Man City", Position = Player.PositionType.Forward, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/418560-1709108116.png?lm=1", Slug = "erling-haaland" },
                    new Player { Name = "Jude Bellingham", Overall = 89, RealTeam = "Real Madrid", Position = Player.PositionType.Midfielder, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/581678-1693987944.jpg?lm=1", Slug = "jude-bellingham" },
                    new Player { Name = "Thibaut Courtois", Overall = 89, RealTeam = "Real Madrid", Position = Player.PositionType.Goalkeeper, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/108390-1717280733.jpg?lm=1", Slug = "thibaut-courtois" },
                    new Player { Name = "Virgil van Dijk", Overall = 88, RealTeam = "Liverpool", Position = Player.PositionType.Defender, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/139208-1702049837.jpg?lm=1", Slug = "virgil-van-dijk" },
                    new Player { Name = "Bukayo Saka", Overall = 87, RealTeam = "Arsenal", Position = Player.PositionType.Midfielder, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/433177-1684155052.jpg?lm=1", Slug = "bukayo-saka" },
                    new Player { Name = "Joshua Kimmich", Overall = 87, RealTeam = "Bayern Munich", Position = Player.PositionType.Midfielder, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/161056-1700039639.jpg?lm=1", Slug = "joshua-kimmich" },
                    new Player { Name = "Marc-André ter Stegen", Overall = 88, RealTeam = "Barcelona", Position = Player.PositionType.Goalkeeper, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/74857-1674465246.jpg?lm=1", Slug = "ter-stegen" },
                    new Player { Name = "Rúben Dias", Overall = 87, RealTeam = "Manchester City", Position = Player.PositionType.Defender, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/258004-1684921271.jpg?lm=1", Slug = "ruben-dias" },
                    new Player { Name = "Marquinhos", Overall = 86, RealTeam = "Paris Saint-Germain", Position = Player.PositionType.Defender, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/181767-1672303747.jpg?lm=1", Slug = "marquinhos" },
                    new Player { Name = "Éder Militão", Overall = 86, RealTeam = "Real Madrid", Position = Player.PositionType.Defender, ImageUrl = "https://img.a.transfermarkt.technology/portrait/header/401530-1719653438.jpg?lm=1", Slug = "eder-militao" }
                };

                context.Players.AddRange(players);
                await context.SaveChangesAsync();
            }
        }
    }
}