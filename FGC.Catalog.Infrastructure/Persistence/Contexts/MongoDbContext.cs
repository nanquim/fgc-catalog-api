using MongoDB.Driver;
using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Infrastructure.Persistence.Contexts;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "fcg";
}

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<GameExtendedInfo> GameExtendedInfos
        => _database.GetCollection<GameExtendedInfo>("game_extended_infos");
}
