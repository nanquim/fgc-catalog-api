namespace FGC.Catalog.Application.Contracts.Events;

public record PaymentProcessedEvent(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    string Status, // "Approved" | "Rejected"
    DateTime ProcessedAt
);
