using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Interfaces;
using Domain.Court;
using Domain.Court.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class CourtEventQueries : ICourtEventQueries
{
    private readonly ApplicationDbContext _context;
    
    public CourtEventQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourtEventCompleteResponse>> GetByClubIdAsync(Guid clubId, DateTime startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var startRange = startDate.Date;
        var endRange = (endDate ?? startDate).Date.AddDays(1).AddTicks(-1);

        var query = _context.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .SelectMany(c => c.CourtEvents
                .Where(e => e.StartTime >= startRange && e.EndTime <= endRange)
                .Select(e => new {Event = e, Court = c}));

        var data = await query.ToListAsync(cancellationToken);
        return data.Select(d => MapToResponse(d.Event, d.Court)).ToList();
    }
    
    
    public async Task<CourtEventCompleteResponse?> GetByIdAsync(Guid courtId, int eventId, CancellationToken cancellationToken)
    { 
        var data = await _context.CourtEvents
            .AsNoTracking()
            .Where(e => e.CourtId == courtId && e.Id == eventId)
            .Join(_context.Courts,
                courtEvent => courtEvent.CourtId,
                court => court.Id,
                (courtEvent, court) => new { courtEvent, court })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
        {
            return null;
        }
        
        return MapToResponse(data.courtEvent, data.court);
    }


    private static CourtEventCompleteResponse MapToResponse(CourtEvent eventCourt, Court court)
    {
        return new CourtEventCompleteResponse(
            eventCourt.Id,
            court.Id,
            court.Name,
            DateTime.SpecifyKind(eventCourt.StartTime, DateTimeKind.Utc),
            DateTime.SpecifyKind(eventCourt.EndTime, DateTimeKind.Utc),
            eventCourt.EventName,
            eventCourt.Description,
            eventCourt.CreatedAt
        );
    }
}