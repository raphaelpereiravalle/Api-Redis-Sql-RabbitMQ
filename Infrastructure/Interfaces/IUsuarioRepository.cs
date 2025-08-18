using Domain.Entities;

namespace Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario> CreateAsync(Usuario usuario);
}