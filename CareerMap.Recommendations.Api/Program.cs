using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string
// Se não vier nada do Azure, usa SQLite por padrão (funciona local e no container)
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["DefaultConnection"]
           ?? "Data Source=/home/site/wwwroot/careermap.db"; // caminho persistente no App Service

// 2) Configuração do EF Core
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    // Usa SQLite sempre (mais simples e confiável pro container)
    options.UseSqlite(conn);
});

// 3) Serviços padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4) Swagger (sempre ativo)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger"; // acessível em /swagger
});

// 5) Pipeline
app.UseAuthorization();
app.MapControllers();

// 6) Healthcheck e rota raiz
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok("API no ar 🚀"));

// 7) Migrations automáticas (SQLite)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}

app.Run();
