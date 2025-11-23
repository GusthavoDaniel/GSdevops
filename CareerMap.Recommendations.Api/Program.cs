using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// 1) CONNECTION STRING
// ----------------------
// Procura primeiro em ConnectionStrings:DefaultConnection (appsettings / App Settings),
// e também aceita VARIABLE "DefaultConnection" pura (App Service -> Configuration).
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? builder.Configuration["DefaultConnection"];

if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException(
        "Defina a connection string 'DefaultConnection' (SQL Server OU SQLite).");

// ----------------------
// 2) PROVEDOR EF CORE
// ----------------------
// Se a conn tiver "Data Source=" ou terminar com .db => SQLite
// Caso contrário => SQL Server.
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    var isSqlite =
        conn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase) ||
        conn.Trim().EndsWith(".db", StringComparison.OrdinalIgnoreCase);

    if (isSqlite)
        options.UseSqlite(conn);
    else
        options.UseSqlServer(conn);
});

// ----------------------
// 3) SERVIÇOS WEB
// ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// ----------------------
// 4) SWAGGER (sempre)
// ----------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap.Recommendations API v1");
    // Deixe o UI em /swagger (padrão). Se quiser na raiz, descomente:
    // c.RoutePrefix = string.Empty;
});

// ----------------------
// 5) PIPELINE
// ----------------------
// O App Service Linux muitas vezes não tem HTTPS ligado no container,
// então só redireciona se houver porta HTTPS definida.
var httpsPorts = Environment.GetEnvironmentVariable("HTTPS_PORTS");
if (!string.IsNullOrEmpty(httpsPorts))
    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ----------------------
// 6) HEALTH + ROOT
// ----------------------
app.MapHealthChecks("/health");

// Raiz aponta pro Swagger pra evitar 404 na home.
app.MapGet("/", () => Results.Redirect("/swagger"));

// ----------------------
// 7) MIGRATIONS AUTOMÁTICAS (opcional)
// ----------------------
// Por padrão só roda em Development. Para forçar em produção, defina
// a App Setting RUN_MIGRATIONS=true.
bool runMigrations =
    app.Environment.IsDevelopment() ||
    string.Equals(Environment.GetEnvironmentVariable("RUN_MIGRATIONS"), "true", StringComparison.OrdinalIgnoreCase);

if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    db.Database.Migrate();
}

app.Run();
