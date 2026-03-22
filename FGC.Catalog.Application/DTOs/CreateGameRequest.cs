namespace FGC.Catalog.Application.DTOs;

public class CreateGameRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
}
