using Domain.Reservation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;
    
    public ReservationRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public void Add(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken)
    {
        return await _context.Reservations
            .Where(r => r.Id == reservationId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetAllReservationsAsync(Guid? userId, Guid? clubId, DateOnly? startDateRange, DateOnly? endDateRange, CancellationToken cancellationToken)
    {
        var query = _context.Reservations.AsNoTracking();

        if (userId.HasValue)
        {
            query = query.Where(r => r.UserId == userId);
        }

        if (clubId.HasValue)
        {
            query = query.Where(r =>r.ClubId == clubId);
        }

        if (startDateRange.HasValue)
        {
            var start = startDateRange;
            var end = endDateRange ?? start;
            
            query = query.Where(r => r.Date >= start && r.Date <= end);
        }
        
        return await query
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetReservationsByCourtIdFilterDate(List<Guid> courtsId, DateOnly date,
        CancellationToken cancellationToken)
    {
        return await _context.Reservations
            .Where(r => courtsId.Contains(r.CourtId) && r.Date == date)
            .ToListAsync(cancellationToken);
    }

    
}