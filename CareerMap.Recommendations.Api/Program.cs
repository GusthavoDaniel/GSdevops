using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Caminho gravável no App Service Linux
var defaultPath = "/home/data/CareerMapRecommendations.db";

// Lê da ConnectionStrings (pega o env var ConnectionStrings__DefaultConnection)
// cai no fallback em /home/data se não existir
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? $"Data Source={defaultPath};Cache=Shared;Pooling=True";

// Garante que a pasta /home/data existe antes de abrir o arquivo
try { System.IO.Directory.CreateDirectory("/home/data"); } catch { /* ignore */ }

// Sempre SQLite
builder.Services.AddDbContext<RecommendationsDbContext>(opt => opt.UseSqlite(conn));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok("API ok 🚀"));

// Cria o arquivo/tabelas se não existir (rápido e suficiente para a demo)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.EnsureCreated(); // usando SQLite
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Falha ao inicializar o banco.");
}

app.Run();
