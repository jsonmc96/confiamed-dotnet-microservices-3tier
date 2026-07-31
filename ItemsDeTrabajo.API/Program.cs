using ItemsDeTrabajo.API.Repositories;
using ItemsDeTrabajo.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// CAPA DE PRESENTACIÓN — Configuración de controladores y documentación
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ─────────────────────────────────────────────────────────────────────────────
// INYECCIÓN DE DEPENDENCIAS — Registro de servicios y repositorios
// ─────────────────────────────────────────────────────────────────────────────

// Capa de Acceso a Datos (Repositories)
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();

// Capa de Lógica de Negocio (Services)
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAsignacionService, AsignacionService>();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────────────────────────────────────

app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
