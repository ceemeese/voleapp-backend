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
    
    public async Task<Court?> GetCourtByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Courts
            .IgnoreQueryFilters()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Court?> GetCourtWithEventsByDateRangeAsync(Guid courtId, DateTime startDate, CancellationToken cancellationToken, DateTime? endDate = null)
    {
        var rangeStart = startDate.Date;
        var rangeEnd = (endDate ?? startDate).Date.AddDays(1).AddTicks(-1);

        var query = _context.Courts
            .Where(c => c.Id == courtId)
            .Include(c => c.CourtEvents
                .Where(e =>
                    e.StartTime >= rangeStart &&
                    e.EndTime <= rangeEnd 
                )
            );
        
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<Court?> GetCourtWithEventByIdAsync(Guid courtId, int eventId, CancellationToken cancellationToken)
    {
        return await _context.Courts
            .Where(c => c.Id == courtId)
            .Include(c => c.CourtEvents.Where(e => e.Id == eventId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Court>> GetCourtsByClubIdAsync(Guid clubId, CancellationToken cancellationToken)
    {       
        return await _context.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameInClubAsync(Guid clubId, string name, CancellationToken cancellationToken)
    {
        return await _context.Courts
            .AnyAsync(c => c.ClubId == clubId && c.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameInClubExcludeIdAsync(Guid clubId, string name, Guid excludeId,CancellationToken cancellationToken)
    {
        return await _context.Courts
            .AnyAsync(c => c.ClubId == clubId && c.Name == name && c.Id != excludeId, cancellationToken);
    }
    
    public void Add(Court court)
    {
        _context.Courts.Add(court);
    }

    public async Task<List<Court>> GetCourtsByClubIdWithFilterEventsAsync(List<Guid> clubId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await _context.Courts
            .Include(c => c.CourtEvents.Where(e =>
                e.StartTime < endDate &&
                e.EndTime > startDate))
            .Where(c => clubId.Contains(c.ClubId) && c.IsActive)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

}