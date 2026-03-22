namespace FGC.Catalog.Domain.Entities;

public class UserLibrary
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime PurchasedAt { get; private set; }

    protected UserLibrary() { }

    public UserLibrary(Guid userId, Guid gameId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        GameId = gameId;
        PurchasedAt = DateTime.UtcNow;
    }
}
