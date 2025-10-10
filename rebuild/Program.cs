using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;
using rebuild.Models.Infra;

// <-- precisa desse using para a sua DAL
// REMOVA isto (está errado):
// using rebuild.Data.DAL.Cadastros.rebuild.Data.DAL.Cadastros;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DbContext (sempre registrar antes de serviços que o usam)
builder.Services.AddDbContext<IESContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("keyum"),b => b.MigrationsAssembly("rebuild")));

// >>> AQUI: registre sua DAL (lifetime Scoped)
builder.Services.AddScoped<InstituicaoDAL>();

builder.Services
    .AddIdentity<UsuarioDaAplicacao, IdentityRole>(options =>
    {
        // opcional: políticas de senha/lockout/etc
        // options.Password.RequiredLength = 6;
        // options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<IESContext>()
    .AddDefaultTokenProviders();
// .AddDefaultUI(); // descomente se estiver usando a UI padrão do Identity

// Cookie de autenticação (Login/AccessDenied personalizados)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Infra/Acessar";
    options.AccessDeniedPath = "/Infra/AcessoNegado";
    // opcional:
    // options.SlidingExpiration = true;
    // options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

var app = builder.Build();

// Seed (opcional) — pode ficar logo após o Build()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IESContext>();
    IESDbInitializer.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
