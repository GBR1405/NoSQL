using EmergencyNow.UI.Models;
using EmergencyNow.UI.Settings;
using Microsoft.AspNetCore.Identity;
using EmergencyNow.UI.LN.Reportes.Crear;
using EmergencyNow.UI.AccesoADatos.Reportes.Crear;
using MongoDB.Driver;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear;
using MongoDbGenericRepository;
using EmergencyNow.UI;
using Microsoft.Extensions.DependencyInjection;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.AccesoADatos.Organizacion;
using EmergencyNow.UI.Abstracciones.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.AccesoADatos.Respuesta;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Respuesta;

var builder = WebApplication.CreateBuilder(args);

// Obtener configuración de MongoDB desde appsettings.json
var mongoDbSettings = builder.Configuration.GetSection("MongoDbConfig").Get<MongoDbConfig>();

// Validar que mongoDbSettings no sea nulo
if (mongoDbSettings == null)
{
    throw new Exception("La configuración de MongoDB no está correctamente definida en appsettings.json.");
}

// Configurar el cliente MongoDB con la cadena de conexión
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
    new MongoClient(mongoDbSettings.ConnectionString));
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
    serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(mongoDbSettings.Name));

builder.Services.AddScoped<Contexto>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var databaseName = "SistemaDeEmergencia"; // Nombre de tu base de datos
    return new Contexto(mongoClient, databaseName);
});

// IngresarArchivosAD
builder.Services.AddTransient<ICrearReporte, CrearReporteAD>();
builder.Services.AddTransient<CrearReporteAD>();
builder.Services.AddTransient<IOrganizacionAD, OrganizacionAD>();
builder.Services.AddTransient<ITipoRespuestaAD, TipoRespuestas>();
builder.Services.AddTransient<IRespuestasAD, RespuestasAD>();

// Configurar servicios de Identity con MongoDB
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbSettings.ConnectionString, mongoDbSettings.Name)
    .AddDefaultTokenProviders();

// Registrar servicios de LN
builder.Services.AddTransient<CrearReporteLN>(); // Registrar CrearReporteLN para la inyección de dependencias

// Agregar Razor Pages
builder.Services.AddRazorPages();

// Agregar controladores con vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapeo de Razor Pages
app.MapRazorPages();

app.Run();
