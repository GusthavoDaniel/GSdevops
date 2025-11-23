using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string: appsettings -> env var -> fallback (arquivo em /home/data)
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["DefaultConnection"]
           ?? "Data Source=/home/data/CareerMapRecommendations.db";

// 2) Garante que a pasta do arquivo existe (Azure App Service Linux grava em /home)
try
{
    var dataSourcePrefix = "Data Source=";
    var idx = conn.IndexOf(dataSourcePrefix, StringComparison.OrdinalIgnoreCase);
    if (idx >= 0)
    {
        var path = conn[(idx + dataSourcePrefix.Length)..].Trim();
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}
catch { /* se falhar, seguimos; o EnsureCreated abaixo acusa no log */ }

// 3) Sempre SQLite
builder.Services.AddDbContext<RecommendationsDbContext>(opt => opt.UseSqlite(conn));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4) Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

// 5) Endpoints
app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok("API ok 🚀"));

// 6) Cria o banco/tabelas se não existir (modo demo)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.EnsureCreated(); // cria o schema para o SQLite
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Falha ao inicializar o banco SQLite.");
}

app.Run();
