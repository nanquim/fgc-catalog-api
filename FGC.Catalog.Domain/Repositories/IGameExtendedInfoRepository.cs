using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Domain.Repositories;

public interface IGameExtendedInfoRepository
{
    Task<GameExtendedInfo?> GetByGameIdAsync(Guid gameId);
    Task UpsertAsync(GameExtendedInfo info);
}
