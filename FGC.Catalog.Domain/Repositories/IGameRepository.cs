using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Domain.Repositories;

public interface IGameRepository
{
    Task AddAsync(Game game);
    Task<Game?> GetByIdAsync(Guid id);
    Task<IEnumerable<Game>> GetAllAsync();
    Task UpdateAsync(Game game);
    Task DeleteAsync(Game game);
}
