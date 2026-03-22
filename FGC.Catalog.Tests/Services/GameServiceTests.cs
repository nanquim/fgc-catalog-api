using FluentAssertions;
using MassTransit;
using Moq;
using FGC.Catalog.Application.Contracts.Events;
using FGC.Catalog.Application.DTOs;
using FGC.Catalog.Application.Services;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;

namespace FGC.Catalog.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _gameService = new GameService(_gameRepositoryMock.Object, _publishEndpointMock.Object);
    }

    // ── CRUD ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Dado_RequestValido_Quando_CreateAsync_Entao_RetornaGuid()
    {
        // Dado
        var request = new CreateGameRequest { Title = "Cyber Quest", Description = "RPG", Price = 49.90m };

        // Quando
        var result = await _gameService.CreateAsync(request);

        // Então
        result.Should().NotBeEmpty();
        _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    public async Task Dado_JogoExistente_Quando_GetAllAsync_Entao_RetornaLista()
    {
        // Dado
        var games = new List<Game> { new Game("Jogo A", "Desc", 10m), new Game("Jogo B", "Desc", 20m) };
        _gameRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

        // Quando
        var result = await _gameService.GetAllAsync();

        // Então
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Dado_IdValido_Quando_GetByIdAsync_Entao_RetornaJogo()
    {
        // Dado
        var game = new Game("Cyber Quest", "RPG", 49.90m);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);

        // Quando
        var result = await _gameService.GetByIdAsync(game.Id);

        // Então
        result.Should().NotBeNull();
        result!.Title.Should().Be("Cyber Quest");
    }

    [Fact]
    public async Task Dado_TituloVazio_Quando_CreateAsync_Entao_LancaArgumentException()
    {
        // Dado
        var request = new CreateGameRequest { Title = "", Description = "Desc", Price = 10m };

        // Quando
        var act = () => _gameService.CreateAsync(request);

        // Então
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Dado_PrecoNegativo_Quando_CreateAsync_Entao_LancaArgumentException()
    {
        // Dado
        var request = new CreateGameRequest { Title = "Jogo", Description = "Desc", Price = -1m };

        // Quando
        var act = () => _gameService.CreateAsync(request);

        // Então
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Preço não pode ser negativo");
    }

    // ── Purchase / OrderPlacedEvent ────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_PurchaseAsync_Entao_PublicaOrderPlacedEvent()
    {
        // Dado
        var game = new Game("Cyber Quest", "RPG", 49.90m);
        var userId = Guid.NewGuid();
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);

        // Quando
        await _gameService.PurchaseAsync(game.Id, userId);

        // Então
        _publishEndpointMock.Verify(
            p => p.Publish(It.Is<OrderPlacedEvent>(e =>
                e.GameId == game.Id &&
                e.UserId == userId &&
                e.Price == game.Price),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_PurchaseAsync_Entao_LancaArgumentException()
    {
        // Dado
        var gameId = Guid.NewGuid();
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

        // Quando
        var act = () => _gameService.PurchaseAsync(gameId, Guid.NewGuid());

        // Então
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Jogo não encontrado");
    }
}
