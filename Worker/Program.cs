using Domain.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var cfg = builder.Configuration.GetValue<string>("Redis:Connection") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(cfg);
});
builder.Services.AddScoped<RedisService>();

// Repos
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Worker service
builder.Services.AddHostedService<WorkerService>();

var host = builder.Build();
host.Run();
