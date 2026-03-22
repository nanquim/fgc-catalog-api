using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;
using FGC.Catalog.Application.DTOs;
using FGC.Catalog.Application.Contracts.Events;
using MassTransit;

namespace FGC.Catalog.Application.Services;

public class GameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public GameService(IGameRepository gameRepository, IPublishEndpoint publishEndpoint)
    {
        _gameRepository = gameRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> CreateAsync(CreateGameRequest request)
    {
        var game = new Game(request.Title, request.Description, request.Price);
        await _gameRepository.AddAsync(game);
        return game.Id;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
        => await _gameRepository.GetAllAsync();

    public async Task<Game?> GetByIdAsync(Guid id)
        => await _gameRepository.GetByIdAsync(id);

    public async Task UpdateAsync(Guid id, UpdateGameRequest request)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Jogo não encontrado");

        game.Update(request.Title, request.Description, request.Price);
        await _gameRepository.UpdateAsync(game);
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Jogo não encontrado");

        await _gameRepository.DeleteAsync(game);
    }

    public async Task PurchaseAsync(Guid gameId, Guid userId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId)
            ?? throw new ArgumentException("Jogo não encontrado");

        await _publishEndpoint.Publish(new OrderPlacedEvent(
            Guid.NewGuid(),
            userId,
            game.Id,
            game.Title,
            game.Price,
            DateTime.UtcNow));
    }
}
