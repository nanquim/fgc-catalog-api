using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FGC.Catalog.Application.DTOs;
using FGC.Catalog.Application.Services;

namespace FGC.Catalog.Api.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateGameRequest request)
    {
        var gameId = await _gameService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = gameId }, new { id = gameId });
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        return game is null ? NotFound() : Ok(game);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateGameRequest request)
    {
        await _gameService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _gameService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{gameId:guid}/purchase")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Purchase(Guid gameId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _gameService.PurchaseAsync(gameId, userId);
        return Accepted(new { message = "Pedido recebido e em processamento" });
    }
}
