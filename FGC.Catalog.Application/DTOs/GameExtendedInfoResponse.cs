namespace FGC.Catalog.Application.DTOs;

public class GameExtendedInfoResponse
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<string> Screenshots { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<string> Platforms { get; set; } = [];
    public double AverageRating { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
}
