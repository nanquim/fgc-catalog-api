using MongoDB.Driver;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;
using FGC.Catalog.Infrastructure.Persistence.Contexts;

namespace FGC.Catalog.Infrastructure.Repositories;

public class GameExtendedInfoRepository : IGameExtendedInfoRepository
{
    private readonly IMongoCollection<GameExtendedInfo> _collection;

    public GameExtendedInfoRepository(MongoDbContext context)
    {
        _collection = context.GameExtendedInfos;
    }

    public async Task<GameExtendedInfo?> GetByGameIdAsync(Guid gameId)
    {
        var filter = Builders<GameExtendedInfo>.Filter.Eq(x => x.GameId, gameId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(GameExtendedInfo info)
    {
        var filter = Builders<GameExtendedInfo>.Filter.Eq(x => x.GameId, info.GameId);
        var options = new ReplaceOptions { IsUpsert = true };

        if (info.Id == Guid.Empty)
            info.Id = Guid.NewGuid();

        await _collection.ReplaceOneAsync(filter, info, options);
    }
}
