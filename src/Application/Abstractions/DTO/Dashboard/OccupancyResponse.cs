namespace Application.Abstractions.DTO.Dashboard;

public sealed record OccupancyResponse(
    IReadOnlyList<DayOccupancyDto> OccupancyByDayOfWeek,
    IReadOnlyList<CourtOccupancyDto> OccupancyByCourt,
    IReadOnlyList<MonthOccupancyDto>  OccupancyEvolution
);

public sealed record DayOccupancyDto(string DayName, double OccupancyRate);
public sealed record CourtOccupancyDto(Guid CourtId, string CourtName, double OccupancyRate);
public sealed record MonthOccupancyDto(string MonthName, int MonthNumber, double OccupancyRate);