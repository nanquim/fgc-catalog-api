using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FGC.Catalog.Domain.Entities;

namespace FGC.Catalog.Infrastructure.Persistence.Configurations;

public class UserLibraryConfiguration : IEntityTypeConfiguration<UserLibrary>
{
    public void Configure(EntityTypeBuilder<UserLibrary> builder)
    {
        builder.HasKey(ul => ul.Id);
        builder.HasIndex(ul => new { ul.UserId, ul.GameId }).IsUnique();
        builder.Property(ul => ul.PurchasedAt).IsRequired();
    }
}
