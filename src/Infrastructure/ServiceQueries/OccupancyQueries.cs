using System.Globalization;
using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Club.Extensions;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Infrastructure.ServiceQueries.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class OccupancyQueries : IOccupancyQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public OccupancyQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private record PeriodReservationReadModel(Guid CourtId, decimal TotalPrice, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime);
    private record PeriodCourtEventReadModel(Guid CourtId, DateTime StartTime, DateTime EndTime);
    private record CourtReadModel(Guid Id, string Name);
    private record ScheduleReadModel(Domain.Club.Enum.DayOfWeek DayOfWeek, TimeOnly OpeningTime, TimeOnly ClosingTime);
    

    public async Task<OccupancyResponse> GetOccupancyStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var currentYear = startDate.Year;
        var startOfYear = new DateOnly(currentYear, 1, 1);
        var endOfYear = new DateOnly(currentYear, 12, 31);
        
        var startOfYearDateTime = startOfYear.ToDateTime(TimeOnly.MinValue);
        var endOfYearDateTime = endOfYear.ToDateTime(TimeOnly.MaxValue);

        var reservations = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.ClubId == clubId && r.Date.Year == currentYear && r.Status != Status.Cancelled)
            .Select(r => new PeriodReservationReadModel(r.CourtId, r.Price.TotalPrice, r.Date, r.StartTime, r.EndTime))
            .ToListAsync(cancellationToken);

        var clubEvents = await _dbContext.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .SelectMany(c => c.CourtEvents)
            .Where(ce => ce.StartTime >= startOfYearDateTime && ce.EndTime <= endOfYearDateTime)
            .Select(ce => new PeriodCourtEventReadModel(ce.CourtId, ce.StartTime, ce.EndTime))
            .ToListAsync(cancellationToken);
        
        var courts = await _dbContext.Courts
            .AsNoTracking()
            .Where(c => c.ClubId == clubId && c.IsActive)
            .Select(c => new CourtReadModel(c.Id, c.Name ))
            .ToListAsync(cancellationToken);
        
        var schedules = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.Id == clubId)
            .SelectMany(c => c.Schedules)
            .Where(s => !s.IsClosed)
            .Select(s => new ScheduleReadModel(s.DayOfWeek, s.OpeningTime, s.ClosingTime ))
            .ToListAsync(cancellationToken);
        
        if (courts.Count == 0 || schedules.Count == 0)
        {
            return new OccupancyResponse(new List<DayOccupancyDto>(), new List<CourtOccupancyDto>(), new List<MonthOccupancyDto>());
        }
        
        var periodReservations = reservations.Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
        
        var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);
        var periodEvents = clubEvents.Where(e => e.StartTime >= startDateTime && e.EndTime <= endDateTime).ToList();
        
        var daysCountInPeriod = OccupancyCalculator.CountDaysOfWeekInPeriod(startDate, endDate);
        
        var occupancyByDay = CalculateOccupancyByDayOfWeek(periodReservations, periodEvents, schedules, daysCountInPeriod, courts.Count);
        var occupancyByCourt = CalculateOccupancyByCourt(periodReservations, periodEvents, schedules, daysCountInPeriod, courts);
        var occupancyEvolution = CalculateAnnualOccupancyEvolution(currentYear, reservations, clubEvents, schedules, courts.Count);

        return new OccupancyResponse(occupancyByDay, occupancyByCourt, occupancyEvolution);
    }

    

    private IReadOnlyList<DayOccupancyDto> CalculateOccupancyByDayOfWeek(
        List<PeriodReservationReadModel> reservations, List<PeriodCourtEventReadModel> events,
        List<ScheduleReadModel> schedules, Dictionary<DayOfWeek, int> daysCount, int totalCourts)
    {
        var temporaryList = new List<(DayOfWeek Day, double Rate)>();
        var schedulesByDay = schedules.GroupBy(s => s.DayOfWeek.ToDotNetDay());
        
        foreach (var group in schedulesByDay)
        {
            DayOfWeek dotnetDay = group.Key;
            if (!daysCount.TryGetValue(dotnetDay, out int occurrencesAtMonth) || occurrencesAtMonth == 0) continue;

            double totalHoursOpenPerDay = 0;
            foreach (var sched in group)
            {
                totalHoursOpenPerDay += (sched.ClosingTime - sched.OpeningTime).TotalHours;
            }
            
            var totalAvailableHours = totalCourts * totalHoursOpenPerDay * occurrencesAtMonth;

            var resHours = reservations.Where(r => r.Date.DayOfWeek == dotnetDay).Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var evHours = events.Where(e => e.StartTime.DayOfWeek == dotnetDay).Sum(e => (e.EndTime - e.StartTime).TotalHours);

            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalAvailableHours);
            temporaryList.Add((dotnetDay, rate));
        }

        return temporaryList
            .OrderBy(item => item.Day == DayOfWeek.Sunday ? 7 : (int)item.Day)
            .Select(item => new DayOccupancyDto(OccupancyCalculator.DayNames[item.Day], item.Rate))
            .ToList();
    }

    private IReadOnlyList<CourtOccupancyDto> CalculateOccupancyByCourt(
        List<PeriodReservationReadModel> reservations, List<PeriodCourtEventReadModel> events,
        List<ScheduleReadModel> schedules, Dictionary<DayOfWeek, int> daysCount, List<CourtReadModel> courts)
    {
        var result = new List<CourtOccupancyDto>();
        double totalOpenHoursPerCourt = 0;

        foreach (var sched in schedules)
        {
            DayOfWeek dotnetDay = sched.DayOfWeek.ToDotNetDay();
            if (daysCount.TryGetValue(dotnetDay, out int occurrences))
            {
                totalOpenHoursPerCourt += (sched.ClosingTime - sched.OpeningTime).TotalHours * occurrences;
            }
        }

        var reservationsByCourt = reservations.ToLookup(r => r.CourtId);
        var eventsByCourt = events.ToLookup(e => e.CourtId);

        foreach (var court in courts)
        {
            double resHours = reservationsByCourt[court.Id].Sum(r => (r.EndTime - r.StartTime).TotalHours);;
            double evHours = eventsByCourt[court.Id].Sum(e => (e.EndTime - e.StartTime).TotalHours);
            
            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalOpenHoursPerCourt);
            result.Add(new CourtOccupancyDto(court.Id, court.Name, rate));
        }

        return result;
    }

    private IReadOnlyList<MonthOccupancyDto> CalculateAnnualOccupancyEvolution(
        int year, List<PeriodReservationReadModel> allReservations, List<PeriodCourtEventReadModel> allEvents,
        List<ScheduleReadModel> schedules, int totalCourts)
    {
        var result = new List<MonthOccupancyDto>();
        var culture = new CultureInfo("es-ES");
        var textInfo = culture.TextInfo;

        var reservationsByMonth = allReservations.ToLookup(r => r.Date.Month);
        var eventsByMonth = allEvents.ToLookup(e => e.StartTime.Month);
        
        for (var m = 1; m <= 12; m++)
        {
            var monthName = textInfo.ToTitleCase(culture.DateTimeFormat.GetMonthName(m));
            
            var startOfMonth = new DateOnly(year, m, 1);
            var endOfMonth = new DateOnly(year, m, DateTime.DaysInMonth(year, m));
            var daysCount = OccupancyCalculator.CountDaysOfWeekInPeriod(startOfMonth, endOfMonth);

            double totalOpenHoursInMonth = 0;
            foreach (var sched in schedules)
            {
                DayOfWeek dotnetDay = sched.DayOfWeek.ToDotNetDay();
                if (daysCount.TryGetValue(dotnetDay, out int occurrences))
                {
                    totalOpenHoursInMonth += (sched.ClosingTime - sched.OpeningTime).TotalHours * occurrences;
                }
            }

            var totalAvailableHoursInMonth = totalOpenHoursInMonth * totalCourts;
            
            var resHours = reservationsByMonth[m].Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var evHours = eventsByMonth[m].Sum(e => (e.EndTime - e.StartTime).TotalHours);
            
            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalAvailableHoursInMonth);
            result.Add(new MonthOccupancyDto(monthName, m, rate));
        }

        return result;
    }
}