using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Domain.Repositories;

public interface IUserLibraryRepository
{
    Task AddAsync(UserLibrary entry);
    Task<IEnumerable<UserLibrary>> GetByUserIdAsync(Guid userId);
}
