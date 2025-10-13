using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;
using rebuild.Data.DAL.Discente;
using rebuild.Data.DAL.Docente;
using rebuild.Models.Infra;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<IESContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("keyum"),
        b => b.MigrationsAssembly("rebuild")));

// DAL
builder.Services.AddScoped<InstituicaoDAL>();
builder.Services.AddScoped<AcademicoDAL>();
builder.Services.AddScoped<DepartamentoDAL>();
builder.Services.AddScoped<ProfessorDAL>();

// >>> SESSION: serviços
builder.Services.AddDistributedMemoryCache(); // provedor em memória
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // tempo de ociosidade
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;              // essencial p/ GDPR
});

// Identity
builder.Services
    .AddIdentity<UsuarioDaAplicacao, IdentityRole>()
    .AddEntityFrameworkStores<IESContext>()
    .AddDefaultTokenProviders();

// Cookie de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Infra/Acessar";
    options.AccessDeniedPath = "/Infra/AcessoNegado";
});

var app = builder.Build();

// Seed (opcional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IESContext>();
    IESDbInitializer.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Erro/Aplicacao");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// >>> SESSION: middleware (antes dos endpoints)
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Home/Error/", "?statusCode={0}");

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();