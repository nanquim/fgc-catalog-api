using Microsoft.EntityFrameworkCore;
using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<UserLibrary> UserLibraries => Set<UserLibrary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
