using GestionDeUsuario.API.Repositories;
using GestionDeUsuario.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// CAPA DE PRESENTACIÓN — Controladores y documentación
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ─────────────────────────────────────────────────────────────────────────────
// INYECCIÓN DE DEPENDENCIAS
// ─────────────────────────────────────────────────────────────────────────────

// Capa de Acceso a Datos
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();

// Capa de Lógica de Negocio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

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
