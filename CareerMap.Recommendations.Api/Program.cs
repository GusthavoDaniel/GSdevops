using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // para IsSqlite()

var builder = WebApplication.CreateBuilder(args);

// Connection string: appsettings -> env var -> fallback (arquivo no /home)
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["DefaultConnection"]
           ?? "Data Source=/home/site/wwwroot/CareerMapRecommendations.db";

// Sempre SQLite
builder.Services.AddDbContext<RecommendationsDbContext>(opt =>
    opt.UseSqlite(conn));

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

// Cria o banco/tabelas se não existir (rápido para SQLite)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();

    // Para não travar por falta de migrations, garante o schema no SQLite
    if (db.Database.IsSqlite())
        db.Database.EnsureCreated(); // simples e imediato para a demo
    else
        db.Database.Migrate();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Falha ao inicializar o banco.");
}

app.Run();
