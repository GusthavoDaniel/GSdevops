using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string (vem de appsettings.* ou das App Settings no Azure)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// 2) Provedor do EF: SQLite no Dev, SQL Server no Azure/Prod
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlite(conn);          // ex.: "Data Source=careermap.db"
    else
        options.UseSqlServer(conn);       // ex.: "Server=tcp:...;Database=...;User ID=...;Password=...;Encrypt=True;"
});

// 3) Serviços padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4) Swagger em todos os ambientes (precisamos no Azure)
app.UseSwagger();
app.UseSwaggerUI();

// 5) Pipeline
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// 6) Healthcheck e rota raiz
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok("API no ar 🚀"));

// 7) Migrations automáticas só no Dev (local)
//   No Azure vamos aplicar com migrations/SQL ou deixar a base já criada.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}

app.Run();
