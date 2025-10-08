using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros; // <-- precisa desse using para a sua DAL
// REMOVA isto (está errado):
// using rebuild.Data.DAL.Cadastros.rebuild.Data.DAL.Cadastros;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext (sempre registrar antes de serviços que o usam)
builder.Services.AddDbContext<IESContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("keyum")));

// >>> AQUI: registre sua DAL (lifetime Scoped)
builder.Services.AddScoped<InstituicaoDAL>();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
