using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Lê a connection string (App Settings -> env var -> fallback local)
var rawConn = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? builder.Configuration["DefaultConnection"]
             ?? "/home/site/wwwroot/CareerMapRecommendations.db";

// Escolhe o provider pelo formato da string
builder.Services.AddDbContext<RecommendationsDbContext>(opt =>
{
    if (!string.IsNullOrWhiteSpace(rawConn) &&
        (rawConn.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
         rawConn.Contains("Data Source=tcp:", StringComparison.OrdinalIgnoreCase)))
    {
        opt.UseSqlServer(rawConn);
    }
    else
    {
        var sqliteConn = rawConn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
            ? rawConn
            : $"Data Source={rawConn}";
        opt.UseSqlite(sqliteConn);
    }
});

// Serviços
builder.Services.AddHealthChecks();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middlewares (não usar HTTPS redirect no Linux container)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

// Endpoints
app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok("API ok 🚀"));

// Migrations no startup
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Falha ao aplicar migrations no startup");
}

app.Run();
