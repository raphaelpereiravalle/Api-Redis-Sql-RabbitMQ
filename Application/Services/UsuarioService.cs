using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Cache;

namespace Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly RedisService _cache;

    public UsuarioService(IUsuarioRepository repo, RedisService cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        var cached = await _cache.GetUsuarioAsync(id);
        if (cached != null) return cached;

        var usuario = await _repo.GetByIdAsync(id);
        if (usuario != null)
            await _cache.SetUsuarioAsync(usuario);

        return usuario;
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        var created = await _repo.CreateAsync(usuario);
        await _cache.SetUsuarioAsync(created);
        return created;
    }
}
