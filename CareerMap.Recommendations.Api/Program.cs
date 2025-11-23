using System.IO;
using CareerMap.Recommendations.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------- 1) Conexão (SQLite em disco persistente do App Service) -------------------
string defaultDbPath = "/home/site/wwwroot/data/careermap.db"; // diretório persistente
Directory.CreateDirectory(Path.GetDirectoryName(defaultDbPath)!);

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["DefaultConnection"]
           ?? $"Data Source={defaultDbPath}";

// ------------------- 2) EF Core (SQLite) -------------------
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
{
    options.UseSqlite(conn);
});

// ------------------- 3) Serviços comuns -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// (Opcional) CORS amplo para testes / Swagger no navegador
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ------------------- 4) Middleware/Pipeline -------------------
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerMap API v1");
    c.RoutePrefix = "swagger"; // /swagger
});

// Se estiver por trás de HTTPS no front, não força redirecionamento aqui
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// Health/readiness rápidos (o App Service costuma pingar "/" ou alguma rota simples)
app.MapHealthChecks("/health", new HealthCheckOptions { });
app.MapGet("/", () => Results.Ok(new
{
    status = "ok",
    message = "API no ar 🚀",
    swagger = "/swagger"
}));
app.MapGet("/__info", () => Results.Ok(new
{
    env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),
    portHint = Environment.GetEnvironmentVariable("WEBSITES_PORT")
}));

// ------------------- 5) Migração do banco -------------------
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    // Cria arquivo/directório se ainda não existir (já garantimos o diretório acima)
    db.Database.Migrate(); // aplica migrations
}
catch (Exception ex)
{
    // Não derruba o app se a migration falhar; registra e tenta ao menos garantir o schema.
    app.Logger.LogError(ex, "Falha ao aplicar migrations. Tentando EnsureCreated...");
    try
    {
        using var scope2 = app.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
        db2.Database.EnsureCreated();
    }
    catch (Exception ex2)
    {
        app.Logger.LogError(ex2, "EnsureCreated também falhou. A API sobe mesmo assim para não quebrar o healthcheck.");
    }
}

// ------------------- 6) Porta/Kestrel -------------------
// No Dockerfile já setamos: ASPNETCORE_URLS=http://+:8080
// Se preferir adaptar automaticamente ao WEBSITES_PORT, descomente abaixo:
//
// var websitesPort = Environment.GetEnvironmentVariable("WEBSITES_PORT");
// if (!string.IsNullOrWhiteSpace(websitesPort) && websitesPort != "8080")
// {
//     app.Logger.LogInformation("Detectado WEBSITES_PORT={Port}. Ouça também nessa porta.", websitesPort);
//     // Em hosting minimal, ASPNETCORE_URLS já resolve. Só logamos para confirmação.
// }

app.Run();
