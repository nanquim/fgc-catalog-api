namespace FGC.Catalog.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }

    protected Game()
    {
        Title = null!;
        Description = null!;
    }

    public Game(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição é obrigatória");

        if (price < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
    }

    public void Update(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição é obrigatória");

        if (price < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        Title = title;
        Description = description;
        Price = price;
    }
}
