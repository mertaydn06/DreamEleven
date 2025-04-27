using DreamEleven.Business.Abstract;
using DreamEleven.Business.Concrete;
using DreamEleven.DataAccess;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.DataAccess.Concrete;
using DreamEleven.Identity;
using DreamEleven.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DreamElevenDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Sql_connection")));  //  DreamElevenDbContext sınıfı SQLite veritabanına bağlanacak şekilde yapılandırılmış olur. Uygulama çalıştırıldığında DreamEleven.db veritabanı kullanılır ve EF Core üzerinden veritabanı işlemleri gerçekleştirilebilir.


// Servisleri dependency injection container'a ekliyoruz
builder.Services.AddScoped<ITeamRepository, EfTeamRepository>();  // DB erişimi için
builder.Services.AddScoped<ITeamService, TeamManager>();          // Controller'a servis için
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerManager>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<ICommentService, CommentManager>();


// Identity servisleri
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DreamElevenDbContext>()
    .AddDefaultTokenProviders();


// Identity ayarları
builder.Services.Configure<IdentityOptions>(options =>
{
    // Şifre gereksinimleri
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    // Kullanıcı adı ve e-posta ayarları
    options.User.RequireUniqueEmail = true; // Her kullanıcının benzersiz bir e-posta adresine sahip olması zorunlu.

    // Giriş / kilitleme ayarları
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Kullanıcı adında yalnızca belirtilen karakterler kullanılabilir.

    // Hesap kilitleme ayarları
    options.Lockout.MaxFailedAccessAttempts = 5; // Kullanıcı 5 kez başarısız giriş yaparsa hesabı geçici olarak kilitler.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // 5 dakika boyunca hesabın kilitli kalmasını sağlar.

    // Giriş ayarları
    options.SignIn.RequireConfirmedEmail = false; // Kullanıcının giriş yapabilmesi için e-posta adresini onaylaması gerekir.

});


// Cookie yapılandırması
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Kullanıcı, yetkilendirilmiş bir sayfaya erişmeye çalıştığında ancak giriş yapmamışsa, onu login sayfasına yönlendirecek olan URL'yi belirtiyoruz.

    options.AccessDeniedPath = "/Account/AccessDenied";  // Eğer kullanıcı sisteme giriş yapmış ancak erişim yetkisi olmayan bir sayfaya gitmeye çalışırsa,  bu durumda ona bir "Erişim Engellendi" sayfası gösterilecek.

    options.SlidingExpiration = true;  // "SlidingExpiration" özelliği true olduğunda, kullanıcı aktifken (yani herhangi bir işlem yaptığında) çerez süresi uzatılır.

    options.ExpireTimeSpan = TimeSpan.FromDays(30);  // Çerezin geçerlilik süresini belirliyoruz. Burada oturum açıldıktan 30 gün sonra çerez geçersiz olacak.
});


builder.Services.AddSession();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();


// Seed Data - Admin ve Roller
using (var scope = app.Services.CreateScope())  // Uygulama kapsamı (scope) oluşturulur. Bu, DI (Dependency Injection) servislerini kullanabilmek için gereklidir.
{
    var services = scope.ServiceProvider;  // DI servis sağlayıcıdan tüm servisleri alıyoruz.
    var context = services.GetRequiredService<DreamElevenDbContext>();

    // RoleManager ve UserManager servislerini alıyoruz.
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Migration işlemi ve admin ve rollerin oluşturulması
    await SeedData.SeedRolesAndAdminAsync(roleManager, userManager, context);
}


app.MapControllerRoute(
    name: "player-details",
    pattern: "player/{slug}",
    defaults: new { controller = "Home", action = "PlayerDetails" });  // .../player/lionel-messi


app.MapControllerRoute(
name: "profile",
pattern: "profile/{username}",
defaults: new { controller = "User", action = "Profile" });  // .../profile/"username"


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();