using System.Globalization;
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
        int totalClubsPeriod = clubs.Count(c => c.CreatedAt >= startDateTime && c.CreatedAt <= endDateTime);
        int totalNewPlayersPeriod = players.Count(u => u.CreatedAt >= startDateTime && u.CreatedAt <= endDateTime);
        
        double averageReservationsPerClub = totalClubsPeriod > 0 
            ? Math.Round((double)periodReservations.Count / totalClubsPeriod, 2) 
            : 0;
        
        var monthlyEvolution = BuildMonthlyEvolution(reservations, clubs, players);
        
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

        var clubs = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.CreatedAt >= startOfYearDt && c.CreatedAt <= endOfYearDt)
            .Select(c => new ClubData(c.Id, c.CreatedAt))
            .ToListAsync(cancellationToken);

        var players = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.CreatedAt >= startOfYearDt && u.CreatedAt <= endOfYearDt)
            .Select(u => new PlayerData(u.Id, u.CreatedAt))
            .ToListAsync(cancellationToken);

        return (reservations, clubs, players);
    }

    private List<GlobalMonthPerformanceDto> BuildMonthlyEvolution(List<ReservationData> reservations, List<ClubData> clubs, List<PlayerData> players)
    {
        var evolution = new List<GlobalMonthPerformanceDto>();

        for (int m = 1; m <= 12; m++)
        {
            var monthRes = reservations.Where(r => r.Date.Month == m).ToList();
            int clubsInMonth = clubs.Count(c => c.CreatedAt.Month == m);
            int playersInMonth = players.Count(u => u.CreatedAt.Month == m);

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