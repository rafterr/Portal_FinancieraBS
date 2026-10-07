using BusinessInterfase;
using BusinessLayer;
using BusinessType;
using DataInterfase;
using DataLayer;
using System.Globalization;
using FinancieraBS.Data;
using FinancieraBS.Models;
using FinancieraBS.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexión viene de User Secrets (local) o de appsettings.Production.json / variables
// de entorno (hosting). Nunca se guarda en el repositorio.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        $"Falta la cadena de conexión 'ConnectionStrings:DefaultConnection' (entorno: {builder.Environment.EnvironmentName}). " +
        "En local configúrela con User Secrets (clic derecho en el proyecto FinancieraBS > Administrar secretos de usuario, " +
        "o 'dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"' dentro de la carpeta FinancieraBS); " +
        "los User Secrets solo se cargan con ASPNETCORE_ENVIRONMENT=Development. " +
        "En el hosting use appsettings.Production.json o variables de entorno. Ver README.md.");
}

builder.Services.AddDbContext<FinancieraContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 25)),
        mySql => mySql.MigrationsAssembly("DataLayer")
    ));

// Configurar Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // Bloqueo tras intentos fallidos
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddErrorDescriber<SpanishIdentityErrorDescriber>()
.AddEntityFrameworkStores<FinancieraContext>()
.AddDefaultTokenProviders();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    // En producción la cookie solo viaja por HTTPS; en local se permite el perfil http
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
});

// Repositorios
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<IClienteRepository, ClienteRepository>();
builder.Services.AddTransient<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddTransient<IpagoRepository, PagoRepository>();
builder.Services.AddTransient<IDocumentoRepository, DocumentoRepository>();

// Procesadores
builder.Services.AddTransient<IUsuarioProcessor, UsuarioProcessor>();
builder.Services.AddTransient<IClienteProcessor, ClienteProcessor>();
builder.Services.AddTransient<IPrestamoProcessor, PrestamoProcessor>();
builder.Services.AddTransient<IPagoProcessor, PagoProcessor>();
builder.Services.AddTransient<IDocumentoProcessor, DocumentoProcessor>();

// Documentos: carpeta privada fuera de wwwroot (no se sirve como archivo estático)
var rutaDocumentos = Path.Combine(builder.Environment.ContentRootPath,
    builder.Configuration["Documentos:RutaAlmacenamiento"] ?? Path.Combine("App_Data", "documentos"));
builder.Services.AddSingleton<IDocumentoStorage>(new LocalDocumentoStorage(rutaDocumentos));
builder.Services.AddSingleton(new DocumentoOptions
{
    TamanoMaximoBytes = builder.Configuration.GetValue("Documentos:TamanoMaximoMB", 5) * 1024L * 1024L
});

// Llaves de cookies/antiforgery persistentes: en hosting compartido evita que se cierren
// las sesiones cada vez que se recicla la aplicación
builder.Services.AddDataProtection()
    .SetApplicationName("FinancieraBS")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));

// Comprobantes PDF (QuestPDF, licencia Community: ingresos anuales menores a USD 1M)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.Configure<NegocioOptions>(builder.Configuration.GetSection("Negocio"));
builder.Services.AddSingleton<IComprobantePdfService, ComprobantePdfService>();

builder.Services.AddControllersWithViews();

// Configurar la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Configuration.GetValue("Database:AplicarMigracionesAlIniciar", true))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<FinancieraContext>().Database.MigrateAsync();
}

await IdentitySeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Formato de moneda, números y fechas de México, sin depender del idioma del servidor
var culturaMx = new CultureInfo("es-MX");
CultureInfo.DefaultThreadCurrentCulture = culturaMx;
CultureInfo.DefaultThreadCurrentUICulture = culturaMx;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culturaMx),
    SupportedCultures = new[] { culturaMx },
    SupportedUICultures = new[] { culturaMx }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
