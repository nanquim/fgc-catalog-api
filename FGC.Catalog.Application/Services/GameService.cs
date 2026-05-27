using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;
using FGC.Catalog.Application.DTOs;
using FGC.Catalog.Application.Contracts.Events;
using FGC.Payments.Application.Contracts.Events;
using MassTransit;

namespace FGC.Catalog.Application.Services;

public class GameService
{
    private const string GamesCacheKey = "games:all";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    };

    private readonly IGameRepository _gameRepository;
    private readonly IGameExtendedInfoRepository _extendedInfoRepository;
    private readonly IDistributedCache _cache;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public GameService(
        IGameRepository gameRepository,
        IGameExtendedInfoRepository extendedInfoRepository,
        IDistributedCache cache,
        ISendEndpointProvider sendEndpointProvider)
    {
        _gameRepository = gameRepository;
        _extendedInfoRepository = extendedInfoRepository;
        _cache = cache;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task<Guid> CreateAsync(CreateGameRequest request)
    {
        var game = new Game(request.Title, request.Description, request.Price);
        await _gameRepository.AddAsync(game);
        await _cache.RemoveAsync(GamesCacheKey);
        return game.Id;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync(GamesCacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<IEnumerable<Game>>(cached) ?? [];

        var games = await _gameRepository.GetAllAsync();
        await _cache.SetStringAsync(GamesCacheKey, JsonSerializer.Serialize(games), CacheOptions);
        return games;
    }

    public async Task<Game?> GetByIdAsync(Guid id)
        => await _gameRepository.GetByIdAsync(id);

    public async Task UpdateAsync(Guid id, UpdateGameRequest request)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Jogo não encontrado");

        game.Update(request.Title, request.Description, request.Price);
        await _gameRepository.UpdateAsync(game);
        await _cache.RemoveAsync(GamesCacheKey);
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new ArgumentException("Jogo não encontrado");

        await _gameRepository.DeleteAsync(game);
        await _cache.RemoveAsync(GamesCacheKey);
    }

    public async Task PurchaseAsync(Guid gameId, Guid userId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId)
            ?? throw new ArgumentException("Jogo não encontrado");

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:order-placed"));
        await endpoint.Send(new OrderPlacedEvent(
            Guid.NewGuid(),
            userId,
            game.Id,
            game.Title,
            game.Price,
            DateTime.UtcNow));
    }

    public async Task<GameExtendedInfoResponse?> GetExtendedInfoAsync(Guid gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game is null) return null;

        var extended = await _extendedInfoRepository.GetByGameIdAsync(gameId);

        return new GameExtendedInfoResponse
        {
            GameId = game.Id,
            Title = game.Title,
            Description = game.Description,
            Price = game.Price,
            Screenshots = extended?.Screenshots ?? [],
            Tags = extended?.Tags ?? [],
            Platforms = extended?.Platforms ?? [],
            AverageRating = extended?.AverageRating ?? 0,
            Publisher = extended?.Publisher ?? string.Empty,
            ReleaseYear = extended?.ReleaseYear ?? 0
        };
    }

    public async Task UpsertExtendedInfoAsync(Guid gameId, UpsertGameExtendedInfoRequest request)
    {
        _ = await _gameRepository.GetByIdAsync(gameId)
            ?? throw new ArgumentException("Jogo não encontrado");

        var info = new GameExtendedInfo
        {
            GameId = gameId,
            Screenshots = request.Screenshots,
            Tags = request.Tags,
            Platforms = request.Platforms,
            AverageRating = request.AverageRating,
            Publisher = request.Publisher,
            ReleaseYear = request.ReleaseYear
        };

        await _extendedInfoRepository.UpsertAsync(info);
    }
}
