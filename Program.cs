using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1) Registrar o DbContext
builder.Services.AddDbContext<IESContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("keyum")));
// ou: AddDbContextPool<AppDbContext>(...) para pooling

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<IESContext>();

    IESDbInitializer.Initialize(db);    // manter se fizer Any() para evitar duplicatas
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
