using System.Globalization;
using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class AnalyticsQueries : IAnalyticsQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public AnalyticsQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnalysisResponse> GetAnalysisStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        int currentYear = startDate.Year;
        
        var startOfYear = new DateOnly(currentYear, 1, 1);
        var endOfYear = new DateOnly(currentYear, 12, 31);
        
        var allYearReservations = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.ClubId == clubId && r.Date >= startOfYear && r.Date <= endOfYear && r.Status != Status.Cancelled)
            .Select(r => new {r.Price.TotalPrice, r.Date})
            .ToListAsync(cancellationToken);
            
        var newUsersCount = await _dbContext.ClubMembers
            .AsNoTracking()
            .CountAsync(cm => cm.ClubId == clubId && cm.RegisteredOn >= startDate && cm.RegisteredOn <= endDate, cancellationToken);

        var periodReservations = allYearReservations
            .Where(r => r.Date >= startDate && r.Date <= endDate)
            .ToList();

        int totalReservationsPeriod = periodReservations.Count;
        decimal totalRevenuePeriod = periodReservations.Sum(r => r.TotalPrice);
        decimal averageTicketPeriod = totalReservationsPeriod > 0 
            ? Math.Round(totalRevenuePeriod / totalReservationsPeriod, 2) 
            : 0;


        var evolutionData = new List<MonthPerformanceDto>();
        var culture= new CultureInfo("es-ES");

        for (int m = 1; m <= 12; m++)
        {
            string monthName = culture.DateTimeFormat.GetMonthName(m);
            monthName = char.ToUpper(monthName[0]) + monthName.Substring(1);

            var monthRes = allYearReservations.Where(r => r.Date.Month == m).ToList();
            
            evolutionData.Add(new MonthPerformanceDto(
                monthName,
                m,
                Math.Round(monthRes.Sum(r => r.TotalPrice), 2),
                monthRes.Count
            ));
        }

        return new AnalysisResponse(totalRevenuePeriod, averageTicketPeriod, totalReservationsPeriod, newUsersCount, evolutionData);
    }
}