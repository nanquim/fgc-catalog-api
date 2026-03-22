using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Title).HasMaxLength(200).IsRequired();
        builder.Property(g => g.Description).HasMaxLength(1000).IsRequired();
        builder.Property(g => g.Price).HasColumnType("decimal(18,2)").IsRequired();
    }
}
