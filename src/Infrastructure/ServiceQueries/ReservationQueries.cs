using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class ReservationQueries : IReservationQueries
{
    private readonly ApplicationDbContext _context;
    
    public ReservationQueries(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }
    
    public async Task<List<ReservationCompleteResponse>> GetAllReservationsAsync(Guid? userId, Guid? clubId, DateOnly? startDateRange, DateOnly? endDateRange, CancellationToken cancellationToken)
    {
        var query = from reservations in _context.Reservations.AsNoTracking()
            join courts in _context.Courts on reservations.CourtId equals courts.Id
            join clubs in _context.Clubs on courts.ClubId equals clubs.Id
            join users in _context.Users on reservations.UserId equals users.Id
            select new { reservations, courts, clubs, users };

        if (userId.HasValue)
        {
            query = query.Where(r => r.reservations.UserId == userId);
        }

        if (clubId.HasValue)
        {
            query = query.Where(r =>r.reservations.ClubId == clubId);
        }

        if (startDateRange.HasValue)
        {
            var start = startDateRange;
            var end = endDateRange ?? start;
            
            query = query.Where(r => r.reservations.Date >= start && r.reservations.Date <= end);
        }

        var orderedReservations = await query
            .OrderByDescending(r => r.reservations.Date)
            .ThenByDescending(r => r.reservations.StartTime)
            .ToListAsync(cancellationToken);
        
        return orderedReservations.Select(x => new ReservationCompleteResponse(
            x.reservations.Id,
            x.reservations.UserId,
            $"{x.users.Name} {x.users.LastName}",
            x.users.Email,
            x.reservations.ClubId,
            x.clubs.Name,
            x.reservations.CourtId,
            x.courts.Name,
            x.reservations.Date,
            x.reservations.StartTime,
            x.reservations.EndTime,
            new StatusResponse((int)x.reservations.Status, x.reservations.Status.ToString()),
            new PriceResponse(
                x.reservations.Price.BasePrice,
                x.reservations.Price.TotalPrice,
                x.reservations.Price.DiscountAmount,
                x.reservations.Price.AppliedDiscountPercent,
                x.reservations.Price.DiscountReason
            ),
            x.reservations.Notes,
            x.reservations.CreatedAt,
            x.reservations.UpdatedAt
            )).ToList();
    }
    
    
    public async Task<ReservationCompleteResponse?> GetReservationCompleteByIdAsync(int reservationId, CancellationToken cancellationToken)
    {
        var query = from reservations in _context.Reservations.AsNoTracking()
            join courts in _context.Courts on reservations.CourtId equals courts.Id
            join clubs in _context.Clubs on courts.ClubId equals clubs.Id
            join users in _context.Users on reservations.UserId equals users.Id
            where reservations.Id == reservationId
            select new { reservations, courts, clubs, users };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return null;
        }
        
        return new ReservationCompleteResponse(
            result.reservations.Id,
            result.reservations.UserId,
            $"{result.users.Name} {result.users.LastName}",
            result.users.Email,
            result.reservations.ClubId,
            result.clubs.Name,
            result.reservations.CourtId,
            result.courts.Name,
            result.reservations.Date,
            result.reservations.StartTime,
            result.reservations.EndTime,
            new StatusResponse((int)result.reservations.Status, result.reservations.Status.ToString()),
            new PriceResponse(
                result.reservations.Price.BasePrice,
                result.reservations.Price.TotalPrice,
                result.reservations.Price.DiscountAmount,
                result.reservations.Price.AppliedDiscountPercent,
                result.reservations.Price.DiscountReason
            ),
            result.reservations.Notes,
            result.reservations.CreatedAt,
            result.reservations.UpdatedAt
        );
    }
}