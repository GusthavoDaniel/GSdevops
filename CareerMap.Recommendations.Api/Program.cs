using System.IO;
using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string (prioriza env var do App Service, depois appsettings)
var rawConn =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? "Data Source=/home/careermap/data/CareerMapRecommendations.db";

// garante que a pasta existe (App Service Linux permite gravar em /home)
var dbPath = rawConn.Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase);
var dbDir = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

// 2) EF Core (SQLite) com alguns tunings p/ evitar lock
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    options.UseSqlite(rawConn, sqlite =>
    {
        // aumente tempo de espera de lock
        sqlite.CommandTimeout(30);
    });
});

// 3) Serviços básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4) Swagger sempre ativo
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger";
});

// 5) Pipeline
app.UseAuthorization();
app.MapControllers();

// 6) Health e raiz
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok("OK"));

// 7) Migrations automáticas com try/catch (não travar startup)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();

        // reduz probabilidade de lock no SQLite
        db.Database.SetCommandTimeout(30);
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // não derruba o app se migrar falhar — loga e segue
        Console.WriteLine($"[Startup][Migrate] Falhou: {ex.Message}");
    }
}

app.Run();
