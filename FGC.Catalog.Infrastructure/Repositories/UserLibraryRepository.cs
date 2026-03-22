using Microsoft.EntityFrameworkCore;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;
using FGC.Catalog.Infrastructure.Persistence.Contexts;

namespace FGC.Catalog.Infrastructure.Repositories;

public class UserLibraryRepository : IUserLibraryRepository
{
    private readonly ApplicationDbContext _context;

    public UserLibraryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserLibrary entry)
    {
        await _context.UserLibraries.AddAsync(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserLibrary>> GetByUserIdAsync(Guid userId)
        => await _context.UserLibraries
            .Where(ul => ul.UserId == userId)
            .ToListAsync();
}
