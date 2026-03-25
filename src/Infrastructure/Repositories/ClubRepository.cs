using Domain.Club;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ClubRepository : IClubRepository
{
    private readonly ApplicationDbContext _context;
    
    public ClubRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Club>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Clubs.ToListAsync(cancellationToken);
    }
    
    public async Task<Club?> GetClubById(Guid clubId, CancellationToken cancellationToken)
    {
        return await _context.Clubs
            .Where(c => c.Id == clubId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Club>> GetAllSearch(string? name, CancellationToken cancellationToken)
    {
        var query = _context.Clubs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<List<Club>> GetAllSearchActive(string? name, CancellationToken cancellationToken)
    {
        var query = _context.Clubs.AsNoTracking().Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    public void Add(Club club)
    {
        _context.Clubs.Add(club);
    }
    
}