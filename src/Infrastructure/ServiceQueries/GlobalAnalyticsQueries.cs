using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Infrastructure.ServiceQueries.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class GlobalAnalyticsQueries : IGlobalAnalyticsQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public GlobalAnalyticsQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private record ReservationData(decimal TotalPrice, DateOnly Date);
    private record ClubData(Guid Id, DateTime CreatedAt);
    private record PlayerData(Guid Id, DateTime CreatedAt);
    
    

    public async Task<GlobalAnalysisResponse> GetGlobalAnalyticsStatsAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var currentYear = startDate.Year;
        
        var (reservations, clubs, players) = await FetchYearlyGlobalDataAsync(currentYear, cancellationToken);
        
        var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);
        
        var periodReservations = reservations.Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
        var totalClubsPeriod = clubs.Count(c => c.CreatedAt >= startDateTime && c.CreatedAt <= endDateTime);
        var totalNewPlayersPeriod = players.Count(u => u.CreatedAt >= startDateTime && u.CreatedAt <= endDateTime);
        
        var totalClubsUpToDate = clubs.Count(c => c.CreatedAt <= endDateTime);
        double averageReservationsPerClub = totalClubsUpToDate > 0 
            ? Math.Round((double)periodReservations.Count / totalClubsUpToDate, 2) 
            : 0;
        
        var monthlyEvolution = BuildMonthlyEvolution(currentYear, reservations, clubs, players);
        
        return new GlobalAnalysisResponse(
            periodReservations.Sum(r => r.TotalPrice),
            totalClubsPeriod,
            periodReservations.Count,
            totalNewPlayersPeriod,
            averageReservationsPerClub,
            monthlyEvolution
        );
    }

    private async Task<(List<ReservationData> Reservations, List<ClubData> Clubs, List<PlayerData> Players)> FetchYearlyGlobalDataAsync(int year, CancellationToken cancellationToken)
    {
        var startOfYear = new DateOnly(year, 1, 1);
        var endOfYear = new DateOnly(year, 12, 31);
        var startOfYearDt = startOfYear.ToDateTime(TimeOnly.MinValue);
        var endOfYearDt = endOfYear.ToDateTime(TimeOnly.MaxValue);

        var reservations = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.Date >= startOfYear && r.Date <= endOfYear && r.Status != Status.Cancelled)
            .Select(r => new ReservationData(r.Price.TotalPrice, r.Date))
            .ToListAsync(cancellationToken);

        /*var clubs = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.CreatedAt >= startOfYearDt && c.CreatedAt <= endOfYearDt)
            .Select(c => new ClubData(c.Id, c.CreatedAt))
            .ToListAsync(cancellationToken);

        var players = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.CreatedAt >= startOfYearDt && u.CreatedAt <= endOfYearDt)
            .Select(u => new PlayerData(u.Id, u.CreatedAt))
            .ToListAsync(cancellationToken);*/
        
        var clubs = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new ClubData(c.Id, c.CreatedAt))
            .ToListAsync(cancellationToken);
        
        var players = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .Select(u => new PlayerData(u.Id, u.CreatedAt))
            .ToListAsync(cancellationToken);

        return (reservations, clubs, players);
    }

    private List<GlobalMonthPerformanceDto> BuildMonthlyEvolution(int year, List<ReservationData> reservations, List<ClubData> clubs, List<PlayerData> players)
    {
        var evolution = new List<GlobalMonthPerformanceDto>();
        var currentMonthReal = DateTime.UtcNow.Month;
        var currentYearReal = DateTime.UtcNow.Year;

        for (int m = 1; m <= 12; m++)
        {
            if (year == currentYearReal && m > currentMonthReal)
            {
                evolution.Add(new GlobalMonthPerformanceDto(
                    AnalyticsHelper.GetCapitalizedMonthName(m),
                    m,
                    0,
                    0,
                    0,
                    0
                ));
                continue;
            }
            var lastSecondOfMonth = new DateTime(year, m, DateTime.DaysInMonth(year, m), 23, 59, 59);
            
            var monthRes = reservations.Where(r => r.Date.Month == m).ToList();
            var clubsInMonth = clubs.Count(c => c.CreatedAt <= lastSecondOfMonth);
            var playersInMonth = players.Count(c => c.CreatedAt <= lastSecondOfMonth);

            evolution.Add(new GlobalMonthPerformanceDto(
                AnalyticsHelper.GetCapitalizedMonthName(m),
                m,
                Math.Round(monthRes.Sum(r => r.TotalPrice), 2),
                monthRes.Count,
                clubsInMonth,
                playersInMonth
            ));
        }

        return evolution;
    }
    
}