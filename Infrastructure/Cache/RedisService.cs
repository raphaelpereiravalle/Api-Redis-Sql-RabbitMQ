using Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Cache;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<Usuario?> GetUsuarioAsync(int id)
    {
        var value = await _db.StringGetAsync($"usuario:{id}");
        if (value.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<Usuario>(value!);
    }

    public async Task SetUsuarioAsync(Usuario usuario)
    {
        var json = JsonSerializer.Serialize(usuario);
        await _db.StringSetAsync($"usuario:{usuario.Id}", json);
    }
}
