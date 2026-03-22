using FluentAssertions;
using MassTransit;
using Moq;
using FGC.Catalog.Application.Contracts.Events;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;
using FGC.Catalog.Infrastructure.Consumers;

namespace FGC.Catalog.Tests.Consumers;

public class PaymentProcessedConsumerTests
{
    private readonly Mock<IUserLibraryRepository> _userLibraryRepositoryMock;
    private readonly PaymentProcessedConsumer _consumer;

    public PaymentProcessedConsumerTests()
    {
        _userLibraryRepositoryMock = new Mock<IUserLibraryRepository>();
        _consumer = new PaymentProcessedConsumer(_userLibraryRepositoryMock.Object);
    }

    [Fact]
    public async Task Dado_StatusApproved_Quando_Consume_Entao_AdicionaJogoNaBiblioteca()
    {
        // Dado
        var ev = new PaymentProcessedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            Status: "Approved",
            ProcessedAt: DateTime.UtcNow);

        var contextMock = new Mock<ConsumeContext<PaymentProcessedEvent>>();
        contextMock.Setup(c => c.Message).Returns(ev);

        // Quando
        await _consumer.Consume(contextMock.Object);

        // Então
        _userLibraryRepositoryMock.Verify(
            r => r.AddAsync(It.Is<UserLibrary>(ul =>
                ul.UserId == ev.UserId &&
                ul.GameId == ev.GameId)),
            Times.Once);
    }

    [Fact]
    public async Task Dado_StatusRejected_Quando_Consume_Entao_NaoAdicionaJogoNaBiblioteca()
    {
        // Dado
        var ev = new PaymentProcessedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            Status: "Rejected",
            ProcessedAt: DateTime.UtcNow);

        var contextMock = new Mock<ConsumeContext<PaymentProcessedEvent>>();
        contextMock.Setup(c => c.Message).Returns(ev);

        // Quando
        await _consumer.Consume(contextMock.Object);

        // Então
        _userLibraryRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<UserLibrary>()),
            Times.Never);
    }
}
