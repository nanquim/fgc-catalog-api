namespace FGC.Catalog.Application.Contracts.Events;

public record OrderPlacedEvent(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    string GameTitle,
    decimal Price,
    DateTime PlacedAt
);
