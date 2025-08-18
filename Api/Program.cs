using Application.Services;
using Domain.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lógica para montar a string de conexão a partir de variáveis de ambiente
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE");

var connectionString = $"Server={dbServer},1433;Database={dbDatabase};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(builder.Configuration["Redis:Connection"]);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddScoped<RedisService>();

// Repos & Services
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

var app = builder.Build();

// Aplica as migrations automaticamente na inicialização da aplicação
// Isso garante que o banco de dados esteja sempre atualizado.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API V1");
        c.RoutePrefix = string.Empty; // Abre o Swagger na raiz /
    });
//}

app.UseAuthorization();
app.MapControllers();

app.Run();
