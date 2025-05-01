using System.Text.Json;
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
            DreamElevenDbContext context,
            IWebHostEnvironment env)
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

            // Admin
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "Admin123*";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true  // Eğer e-posta doğrulaması zorunlu değilse
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    adminUser.Image = "/images/users/User.jpg";
                    await userManager.UpdateAsync(adminUser);
                }
            }

            // Normal kullanıcı
            const string username = "mertaydn";
            const string userEmail = "user@gmail.com";
            const string userPassword = "User123*";

            var appUser = await userManager.FindByEmailAsync(userEmail);

            if (appUser == null)
            {
                appUser = new User
                {
                    UserName = username,
                    Email = userEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(appUser, userPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appUser, "User");

                    appUser.Image = "/images/users/User.jpg";
                    await userManager.UpdateAsync(appUser);
                }
            }

            // Oyuncular JSON'dan yüklenecek
            string filePath = Path.Combine(env.WebRootPath, "data", "players.json");
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                var players = JsonSerializer.Deserialize<List<Player>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new PositionTypeConverter() } // Enum için konverter ekledik
                });

                if (players != null)
                {
                    context.Players.AddRange(players);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
