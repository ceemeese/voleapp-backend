using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Infrastructure.ServiceQueries.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class AnalyticsQueries : IAnalyticsQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public AnalyticsQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private record ClubReservationData(decimal TotalPrice, DateOnly Date);

    public async Task<AnalysisResponse> GetAnalysisStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var currentYear = startDate.Year;
        
        var allYearReservations = await FetchYearlyClubReservationsAsync(clubId, currentYear, cancellationToken);
        var newUsersCount = await CountNewClubMembersAsync(clubId, startDate, endDate, cancellationToken);
        
        var periodReservations = allYearReservations.Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
        var totalReservationsPeriod = periodReservations.Count;
        decimal totalRevenuePeriod = periodReservations.Sum(r => r.TotalPrice);
        
        decimal averageTicketPeriod = totalReservationsPeriod > 0 
            ? Math.Round(totalRevenuePeriod / totalReservationsPeriod, 2) 
            : 0;
        
        var evolutionData = BuildMonthlyEvolution(allYearReservations);

        return new AnalysisResponse(totalRevenuePeriod, averageTicketPeriod, totalReservationsPeriod, newUsersCount, evolutionData);
    }

    private async Task<List<ClubReservationData>> FetchYearlyClubReservationsAsync(Guid clubId, int year, CancellationToken cancellationToken)
    {
        var startOfYear = new DateOnly(year, 1, 1);
        var endOfYear = new DateOnly(year, 12, 31);

        return await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.ClubId == clubId && r.Date >= startOfYear && r.Date <= endOfYear && r.Status != Status.Cancelled)
            .Select(r => new ClubReservationData(r.Price.TotalPrice, r.Date))
            .ToListAsync(cancellationToken);
    }

    private async Task<int> CountNewClubMembersAsync(Guid clubId, DateOnly start, DateOnly end, CancellationToken cancellationToken)
    {
        return await _dbContext.ClubMembers
            .AsNoTracking()
            .CountAsync(cm => cm.ClubId == clubId && cm.RegisteredOn >= start && cm.RegisteredOn <= end, cancellationToken);
    }

    private List<MonthPerformanceDto> BuildMonthlyEvolution(List<ClubReservationData> reservations)
    {
        var evolution = new List<MonthPerformanceDto>();

        for (int m = 1; m <= 12; m++)
        {
            var monthRes = reservations.Where(r => r.Date.Month == m).ToList();

            evolution.Add(new MonthPerformanceDto(
                AnalyticsHelper.GetCapitalizedMonthName(m),
                m,
                Math.Round(monthRes.Sum(r => r.TotalPrice), 2),
                monthRes.Count
            ));
        }

        return evolution;
    }
    
}