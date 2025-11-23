using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string (prioriza variável de ambiente do App Service)
var conn =
    builder.Configuration["ConnectionStrings:DefaultConnection"] // ex.: ConnectionStrings__DefaultConnection
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=/home/site/wwwroot/CareerMapRecommendations.db"; // caminho gravável no App Service

// 2) EF Core (SQLite)
builder.Services.AddDbContext<RecommendationsDbContext>(opt => opt.UseSqlite(conn));

// 3) Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4) Swagger SEMPRE ativo (útil para prod no App Service)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger";
});

// 5) NÃO usar HTTPS redirection dentro do container (o App Service já termina TLS)
//// app.UseHttpsRedirection();

app.UseAuthorization();

// 6) Endpoints
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok("API no ar 🚀"));

// 7) Migrations automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}

app.Run();
