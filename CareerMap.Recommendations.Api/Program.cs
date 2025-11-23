using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Pega a connection string (funciona local e no Azure)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Usa SQLite localmente e SQL Server no Azure
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlite(conn);
    else
        options.UseSqlServer(conn);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Habilita Swagger em todos os ambientes (pra testar no Azure também)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Teste simples pra confirmar que a API subiu
app.MapGet("/", () => Results.Ok("API no ar 🚀"));

// Aplica migrations localmente apenas
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}

app.Run();
