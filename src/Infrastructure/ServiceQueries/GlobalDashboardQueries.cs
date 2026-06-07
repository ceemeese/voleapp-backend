using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Club.Extensions;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class GlobalDashboardQueries : IGlobalDashboardQueries
{
    private readonly ApplicationDbContext _dbContext;

    public GlobalDashboardQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GlobalDashboardResponse> GetGlobalDashboardStatsAsync(CancellationToken cancellationToken)
    {
        var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
        var todayStart = DateTime.Today;
        var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);
        var domainDayOfWeek = DateTime.Today.DayOfWeek.ToDomainDay();

        var totalClubs = await _dbContext.Clubs
            .Where(c => c.IsActive)
            .CountAsync(cancellationToken);

        var totalPlayers = await _dbContext.Users
            .Where(u => u.IsActive)
            .CountAsync(cancellationToken);

        var todayClubActivities = await (
            from r in _dbContext.Reservations
            where r.Date == todayDateOnly && r.Status != Status.Cancelled
            join c in _dbContext.Clubs on r.ClubId equals c.Id
            group r by c.Name
            into g
            select new
            {
                clubName = g.Key,
                ReservationsCount = g.Count()
            }
        ).ToListAsync(cancellationToken);

        var todayReservations = todayClubActivities.Sum(c => c.ReservationsCount);

        var activeClubsToday = todayClubActivities.Count;

        var topClub = todayClubActivities
            .OrderByDescending(c => c.ReservationsCount)
            .FirstOrDefault();
        var topClubName = topClub?.clubName ?? "Sin actividad";

        var globalOccupancyRate = await CalculateGlobalOccupancyAsync(
            todayDateOnly, 
            todayStart, 
            todayEnd, 
            domainDayOfWeek,
            cancellationToken);
        
        return new GlobalDashboardResponse(
            activeClubsToday,
            todayReservations,
            globalOccupancyRate,
            totalClubs,
            totalPlayers,
            topClubName
        );
    }


    private async Task<double> CalculateGlobalOccupancyAsync(DateOnly todayDateOnly, DateTime todayStart, DateTime todayEnd,
        Domain.Club.Enum.DayOfWeek domainDayOfWeek, CancellationToken cancellationToken)
    {
        var reservationsHours = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.Date == todayDateOnly && r.Status != Status.Cancelled)
            .Select(r => new { r.StartTime, r.EndTime })
            .ToListAsync(cancellationToken);
        
        var totalHoursReservations = reservationsHours
            .Sum(r => (r.EndTime - r.StartTime).TotalHours);

        var reservationHours = await _dbContext.CourtEvents
            .AsNoTracking()
            .Where(ce => ce.StartTime >= todayStart && ce.EndTime <= todayEnd)
            .Select(ce => new { ce.StartTime, ce.EndTime })
            .ToListAsync(cancellationToken);

        var totalHoursEvents = reservationHours
            .Sum(r => (r.EndTime - r.StartTime).TotalHours);
        
        var totalHoursConsumed = totalHoursReservations + totalHoursEvents;
        if (totalHoursConsumed == 0) return 0;

        var clubsCapacity = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new
            {
                TotalCourts = _dbContext.Courts.Where(court => court.IsActive).Count(court => court.ClubId == c.Id),
                TotalHoursOpenToday = c.Schedules
                    .Where(s => s.DayOfWeek == domainDayOfWeek && !s.IsClosed)
                    .Select(s => (s.ClosingTime - s.OpeningTime).TotalHours)
                    .Sum()
            })
            .ToListAsync(cancellationToken);

        double totalAvailableHours = clubsCapacity
            .Where(club => club.TotalCourts > 0 && club.TotalHoursOpenToday > 0)
            .Sum(club => club.TotalCourts * club.TotalHoursOpenToday);

        if (totalAvailableHours <= 0) return 0;

        return Math.Round((totalHoursConsumed / totalAvailableHours) * 100, 2);
    }
}