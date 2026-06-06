using System.Globalization;
using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Interfaces.Queries;
using Domain.Club.Extensions;
using Domain.Reservation.Enum;
using Infrastructure.Persistence;
using Infrastructure.ServiceQueries.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class GlobalOccupancyQueries : IGlobalOccupancyQueries
{
    private readonly ApplicationDbContext _dbContext;
    
    public GlobalOccupancyQueries(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private record GlobalPeriodReservationReadModel(Guid ClubId, Guid CourtId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime);
    private record GlobalPeriodCourtEventReadModel(Guid ClubId, Guid CourtId, DateTime StartTime, DateTime EndTime);
    private record ClubReadModel(Guid Id, string Name, int ActiveCourtsCount);
    private record ScheduleReadModel(Guid ClubId, Domain.Club.Enum.DayOfWeek DayOfWeek, TimeOnly OpeningTime, TimeOnly ClosingTime);

    public async Task<GlobalOccupancyResponse> GetGlocalOccupancyStatsAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var currentYear = startDate.Year;
        var startOfYear = new DateOnly(currentYear, 1, 1);
        var endOfYear = new DateOnly(currentYear, 12, 31);
        
        var startOfYearDateTime = startOfYear.ToDateTime(TimeOnly.MinValue);
        var endOfYearDateTime = endOfYear.ToDateTime(TimeOnly.MaxValue);
        
        var clubs = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new ClubReadModel(
                c.Id, 
                c.Name, 
                _dbContext.Courts.Count(court => court.ClubId == c.Id && court.IsActive)))
            .ToListAsync(cancellationToken);
        
        var schedules = await _dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.IsActive)
            .SelectMany(c => c.Schedules)
            .Where(s => !s.IsClosed)
            .Select(s => new ScheduleReadModel(s.ClubId, s.DayOfWeek, s.OpeningTime, s.ClosingTime))
            .ToListAsync(cancellationToken);
        
        if (clubs.Count == 0 || schedules.Count == 0)
        {
            return new GlobalOccupancyResponse(new List<DayOccupancyDto>(), new List<GlobalClubOccupancyDto>(), new List<MonthOccupancyDto>());
        }
        
        var activeClubIds = clubs.Select(c => c.Id).ToList();
        var allReservations = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => activeClubIds.Contains(r.ClubId) && r.Date.Year == currentYear && r.Status != Status.Cancelled)
            .Select(r => new GlobalPeriodReservationReadModel(r.ClubId, r.CourtId, r.Date, r.StartTime, r.EndTime))
            .ToListAsync(cancellationToken);
        
        var allEvents = await (
            from court in _dbContext.Courts.AsNoTracking()
            where activeClubIds.Contains(court.ClubId) && court.IsActive
            join ce in _dbContext.CourtEvents.AsNoTracking() on court.Id equals ce.CourtId
            where ce.StartTime >= startOfYearDateTime && ce.EndTime <= endOfYearDateTime
            select new GlobalPeriodCourtEventReadModel(
                court.ClubId,
                ce.CourtId,     
                ce.StartTime,   
                ce.EndTime       
            )
        ).ToListAsync(cancellationToken);
        
        var periodReservations = allReservations.Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
        
        var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);
        var periodEvents = allEvents.Where(e => e.StartTime >= startDateTime && e.EndTime <= endDateTime).ToList();

        var daysCountInPeriod = OccupancyCalculator.CountDaysOfWeekInPeriod(startDate, endDate);
        
        var occupancyByDay = CalculateGlobalOccupancyByDayOfWeek(periodReservations, periodEvents, schedules, daysCountInPeriod, clubs);
        var occupancyByClub = CalculateGlobalOccupancyByClub(periodReservations, periodEvents, schedules, daysCountInPeriod, clubs);
        var occupancyEvolution = CalculateGlobalAnnualOccupancyEvolution(currentYear, allReservations, allEvents, schedules, clubs);

        return new GlobalOccupancyResponse(occupancyByDay, occupancyByClub, occupancyEvolution);
    }
    
    private IReadOnlyList<DayOccupancyDto> CalculateGlobalOccupancyByDayOfWeek(
        List<GlobalPeriodReservationReadModel> reservations, List<GlobalPeriodCourtEventReadModel> events,
        List<ScheduleReadModel> schedules, Dictionary<DayOfWeek, int> daysCount, List<ClubReadModel> clubs)
    {
        var temporaryList = new List<(DayOfWeek Day, double Rate)>();
        var schedulesByDay = schedules.GroupBy(s => s.DayOfWeek.ToDotNetDay());

        foreach (var group in schedulesByDay)
        {
            DayOfWeek dotnetDay = group.Key;
            if (!daysCount.TryGetValue(dotnetDay, out int occurrences) || occurrences == 0) continue;

            double totalAvailableHoursAllClubs = 0;
            
            foreach (var club in clubs)
            {
                var clubSchedulesForDay = group.Where(s => s.ClubId == club.Id);
                double openHours = clubSchedulesForDay.Sum(s => (s.ClosingTime - s.OpeningTime).TotalHours);
                totalAvailableHoursAllClubs += club.ActiveCourtsCount * openHours * occurrences;
            }

            var resHours = reservations.Where(r => r.Date.DayOfWeek == dotnetDay).Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var evHours = events.Where(e => e.StartTime.DayOfWeek == dotnetDay).Sum(e => (e.EndTime - e.StartTime).TotalHours);

            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalAvailableHoursAllClubs);
            temporaryList.Add((dotnetDay, rate));
        }

        return temporaryList
            .OrderBy(item => item.Day == DayOfWeek.Sunday ? 7 : (int)item.Day)
            .Select(item => new DayOccupancyDto(OccupancyCalculator.DayNames[item.Day], item.Rate))
            .ToList();
    }

    private IReadOnlyList<GlobalClubOccupancyDto> CalculateGlobalOccupancyByClub(
        List<GlobalPeriodReservationReadModel> reservations, List<GlobalPeriodCourtEventReadModel> events,
        List<ScheduleReadModel> schedules, Dictionary<DayOfWeek, int> daysCount, List<ClubReadModel> clubs)
    {
        var result = new List<GlobalClubOccupancyDto>();

        foreach (var club in clubs)
        {
            double totalOpenHoursPerClub = 0;
            var clubSchedules = schedules.Where(s => s.ClubId == club.Id);

            foreach (var sched in clubSchedules)
            {
                DayOfWeek dotnetDay = sched.DayOfWeek.ToDotNetDay();
                if (daysCount.TryGetValue(dotnetDay, out int occurrences))
                {
                    totalOpenHoursPerClub += (sched.ClosingTime - sched.OpeningTime).TotalHours * occurrences;
                }
            }

            var totalAvailableHours = totalOpenHoursPerClub * club.ActiveCourtsCount;
            
            var resHours = reservations.Where(r => r.ClubId == club.Id).Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var evHours = events.Where(e => e.ClubId == club.Id).Sum(e => (e.EndTime - e.StartTime).TotalHours);

            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalAvailableHours);
            result.Add(new GlobalClubOccupancyDto(club.Id, club.Name, rate));
        }
        
        return result.OrderByDescending(c => c.OccupancyRate).ToList();
    }

    private IReadOnlyList<MonthOccupancyDto> CalculateGlobalAnnualOccupancyEvolution(
        int year, List<GlobalPeriodReservationReadModel> allReservations, List<GlobalPeriodCourtEventReadModel> allEvents,
        List<ScheduleReadModel> schedules, List<ClubReadModel> clubs)
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

            double totalAvailableHoursInMonthAllClubs = 0;

            foreach (var club in clubs)
            {
                var clubSchedules = schedules.Where(s => s.ClubId == club.Id);
                double totalOpenHoursInMonth = 0;

                foreach (var sched in clubSchedules)
                {
                    DayOfWeek dotnetDay = sched.DayOfWeek.ToDotNetDay();
                    if (daysCount.TryGetValue(dotnetDay, out int occurrences))
                    {
                        totalOpenHoursInMonth += (sched.ClosingTime - sched.OpeningTime).TotalHours * occurrences;
                    }
                }

                totalAvailableHoursInMonthAllClubs += totalOpenHoursInMonth * club.ActiveCourtsCount;
            }

            var resHours = reservationsByMonth[m].Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var evHours = eventsByMonth[m].Sum(e => (e.EndTime - e.StartTime).TotalHours);

            var rate = OccupancyCalculator.CalculateRate(resHours + evHours, totalAvailableHoursInMonthAllClubs);
            result.Add(new MonthOccupancyDto(monthName, m, rate));
        }

        return result;
    }
}