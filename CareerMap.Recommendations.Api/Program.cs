using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ✅ Caminho do banco — tenta pegar do ambiente, senão usa padrão
var dbPath = Environment.GetEnvironmentVariable("SQLITE_DB_PATH")
             ?? "/home/data/CareerMapRecommendations.db";

// ✅ Garante que a pasta existe (SQLite não cria pastas automaticamente)
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

// ✅ Registra o contexto com SQLite
builder.Services.AddDbContext<RecommendationsDbContext>(opts =>
    opts.UseSqlite($"Data Source={dbPath};Cache=Shared"));

// Serviços padrão
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
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

// ✅ Cria/migra o banco automaticamente
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.EnsureCreated(); // Para SQLite, é o suficiente
    // db.Database.Migrate(); // use se quiser migrations
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Falha ao inicializar o banco.");
}

app.Run();
