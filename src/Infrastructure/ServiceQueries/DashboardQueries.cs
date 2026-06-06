using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Club.Extensions;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class DashboardQueries : IDashboardQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public DashboardQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private record ReservationReadModel(decimal TotalPrice, TimeOnly StartTime, TimeOnly EndTime);
    private record CourtEventReadModel(DateTime StartTime, DateTime EndTime);

    public async Task<ClubDashboardResponse> GetDashboardStatsAsync(Guid clubId, CancellationToken cancellationToken)
    {
        var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);

        var todayStart = DateTime.Today;
        var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);
        
        var clubCourts = await _dbContext.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var todayReservations = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.ClubId == clubId && r.Date == todayDateOnly && r.Status != Status.Cancelled)
            .Select(r => new ReservationReadModel(r.Price.TotalPrice, r.StartTime, r.EndTime ))
            .ToListAsync(cancellationToken);

        var todayClubEvents = await _dbContext.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .SelectMany(c => c.CourtEvents)
            .Where(ce => ce.StartTime >= todayStart && ce.EndTime <= todayEnd)
            .Select(ce => new CourtEventReadModel( ce.StartTime, ce.EndTime ))
            .ToListAsync(cancellationToken);

        var cancelledToday = await _dbContext.Reservations
            .AsNoTracking()
            .CountAsync(r => r.ClubId == clubId && r.Date == todayDateOnly && r.Status == Status.Cancelled, cancellationToken);
        
        var (todayRevenue, averageTicket) = CalculateFinancials(todayReservations);
        
        double globalOccupancyRate = await CalculateOccupancyAsync(
            clubId, 
            clubCourts.Count, 
            todayReservations, 
            todayClubEvents, 
            cancellationToken);
        
        string peakHourToday = CalculatePeakHour(todayReservations, todayClubEvents);
        
        return new ClubDashboardResponse(
            todayRevenue,
            averageTicket,
            todayReservations.Count,
            todayClubEvents.Count,
            globalOccupancyRate,
            cancelledToday,
            peakHourToday
        );
    }
    
    
    
    private (decimal TotalRevenue, decimal AverageTicket) CalculateFinancials(IReadOnlyList<ReservationReadModel> reservations)
    {
        decimal totalRevenue = 0;
        foreach (var r in reservations)
        {
            totalRevenue += r.TotalPrice;
        }

        decimal averageTicket = reservations.Count > 0 ? totalRevenue / reservations.Count : 0;

        return (totalRevenue, averageTicket);
    }
    
    
    private async Task<double> CalculateOccupancyAsync(Guid clubId, int totalCourts, IReadOnlyList<ReservationReadModel> todayReservations, IReadOnlyList<CourtEventReadModel> todayClubEvents, CancellationToken cancellationToken)
    {
        if (totalCourts == 0) return 0;

        var domainDayOfWeek = DateTime.Today.DayOfWeek.ToDomainDay();
        
        var schedule = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.Id == clubId)
            .SelectMany(c => c.Schedules)
            .Where(s => s.DayOfWeek == domainDayOfWeek && !s.IsClosed)
            .Select(s => new { s.OpeningTime, s.ClosingTime })
            .FirstOrDefaultAsync(cancellationToken);

        if (schedule is null) return 0;

        var hoursOpenToday = (schedule.ClosingTime - schedule.OpeningTime).TotalHours;
        
        var totalAvailableCourtHours = totalCourts * hoursOpenToday;
        
        if (totalAvailableCourtHours <= 0) return 0;

        double hoursConsumedByReservations = 0;
        foreach (var r in todayReservations)
        {
            hoursConsumedByReservations += (r.EndTime - r.StartTime).TotalHours;
        }
        
        double hoursConsumedByEvents = 0;
        foreach (var e in todayClubEvents)
        {
            hoursConsumedByEvents += (e.EndTime - e.StartTime).TotalHours;
        }
        
        var totalOccupiedCourtHours = hoursConsumedByReservations + hoursConsumedByEvents;
        
        return Math.Round((totalOccupiedCourtHours / totalAvailableCourtHours) * 100, 2);
    }
    
    
    
    private string CalculatePeakHour(IReadOnlyList<ReservationReadModel> reservations, IReadOnlyList<CourtEventReadModel> clubEvents)
    {
        var allStarts = new List<string>();
        
        foreach (var r in reservations)
        {
            allStarts.Add(r.StartTime.ToString("HH:mm"));
        }

        foreach (var e in clubEvents)
        {
            allStarts.Add(e.StartTime.ToString("HH:mm"));
        }

        return allStarts
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "Sin actividad";
    }
}