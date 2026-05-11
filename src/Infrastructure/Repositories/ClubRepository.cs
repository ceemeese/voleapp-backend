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
    
    public async Task<List<Club>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Clubs.ToListAsync(cancellationToken);
    }
    
    public async Task<Club?> GetClubByIdAsync(Guid clubId, CancellationToken cancellationToken)
    {
        return await _context.Clubs
            .Where(c => c.Id == clubId)
            .Include(c => c.Schedules)
            .Include(c => c.PricingConfig)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<Club?> GetClubWithMembersAsync(Guid clubId, CancellationToken cancellationToken)
    {
        return await _context.Clubs
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == clubId, cancellationToken);
    }

    public async Task<List<Club>> GetAllSearchAsync(string? name, CancellationToken cancellationToken)
    {
        var query = _context.Clubs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<List<Club>> GetAllSearchActiveAsync(string? name, CancellationToken cancellationToken)
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

    public async Task<List<Club>> GetActiveClubsByCityWithSchedulesAsync(string city, CancellationToken cancellationToken)
    {
        return await _context.Clubs
            .Include(c => c.Schedules)
            .Where(c => c.IsActive && c.Address.City == city)
            .ToListAsync(cancellationToken);
    }
}