using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;
using rebuild.Data.DAL.Discente;
using rebuild.Data.DAL.Docente;
using rebuild.Models;
using rebuild.Models.Infra;

var builder = WebApplication.CreateBuilder(args);

// MVC + mensagens de ModelBinding em PT-BR
builder.Services
    .AddControllersWithViews(options =>
    {
        var p = options.ModelBindingMessageProvider;
        p.SetAttemptedValueIsInvalidAccessor((val, field) =>
            $"O valor '{val}' n�o � v�lido para {field}.");
        p.SetNonPropertyAttemptedValueIsInvalidAccessor(val =>
            $"O valor '{val}' n�o � v�lido.");
        p.SetMissingBindRequiredValueAccessor(field =>
            $"O campo {field} � obrigat�rio.");
        p.SetMissingKeyOrValueAccessor(() => "Campo obrigat�rio.");
        p.SetUnknownValueIsInvalidAccessor(field =>
            $"O valor informado � inv�lido para {field}.");
        p.SetValueIsInvalidAccessor(val =>
            $"O valor '{val}' n�o � v�lido.");
        p.SetValueMustBeANumberAccessor(field =>
            $"O campo {field} deve ser num�rico.");
        p.SetValueMustNotBeNullAccessor(field =>
            $"O campo {field} � obrigat�rio.");
    });
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<IESContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("keyum"),
        b => b.MigrationsAssembly("rebuild")));

// DALs
builder.Services.AddScoped<InstituicaoDAL>();
builder.Services.AddScoped<AcademicoDAL>();
builder.Services.AddScoped<DepartamentoDAL>();
builder.Services.AddScoped<ProfessorDAL>();
builder.Services.AddScoped<CursoDAL>();

// SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Identity (um �nico encadeamento)
builder.Services
    .AddIdentity<UsuarioDaAplicacao, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        // options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<IESContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PortugueseIdentityErrorDescriber>(); // mensagens do Identity em PT-BR

// Cookie de autentica��o
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Infra/Acessar";
    options.AccessDeniedPath = "/Infra/AcessoNegado";
});

// ----- Localiza��o pt-BR para datas/n�meros -----
var supportedCultures = new[] { new CultureInfo("pt-BR") };
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    opts.DefaultRequestCulture = new RequestCulture("pt-BR");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
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

// Localiza��o deve vir antes de MVC
app.UseRequestLocalization();

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

// Se estiver usando p�ginas do Identity/razor:
// app.MapRazorPages();

app.Run();