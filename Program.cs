using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1) Registrar o DbContext
builder.Services.AddDbContext<IESContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ou: AddDbContextPool<AppDbContext>(...) para pooling

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IESContext>();
        // aplica migrações (ou use EnsureCreated se preferir)
        context.Database.Migrate();
        // popula dados iniciais (seu inicializador)
        IESDbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao criar/popular a base de dados.");
    }
}

app.MapGet("/Departamento", async (IESContext db) => await db.Departamento.ToListAsync());
app.MapPost("/Departamento", async (IESContext db, Departamento p) =>
{
    db.Departamento.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/Departamento/{p.DepartamentoID}", p);
});

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IESContext>();
    db.Database.Migrate(); // cria/atualiza as tabelas no startup
}

app.Run();
