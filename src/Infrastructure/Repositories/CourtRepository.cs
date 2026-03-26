using Domain.Court;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class CourtRepository : ICourtRepository
{
    private readonly ApplicationDbContext _context;
    
    public CourtRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Court>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Courts.AsNoTracking().ToListAsync(cancellationToken);
    }
    
    public async Task<Court?> GetCourtById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Courts
            .IgnoreQueryFilters()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<List<Court>> GetCourtsByClubId(Guid clubId, CancellationToken cancellationToken)
    {       
        return await _context.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameInClub(Guid clubId, string name, CancellationToken cancellationToken)
    {
        return await _context.Courts
            .AnyAsync(c => c.ClubId == clubId && c.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameInClubExcludeId(Guid clubId, string name, Guid excludeId,CancellationToken cancellationToken)
    {
        return await _context.Courts
            .AnyAsync(c => c.ClubId == clubId && c.Name == name && c.Id != excludeId, cancellationToken);
    }
    
    public void Add(Court court)
    {
        _context.Courts.Add(court);
    }
    
}