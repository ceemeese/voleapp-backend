namespace Application.Abstractions.DTO.Dashboard;

public sealed record GlobalOccupancyResponse(
    IReadOnlyList<DayOccupancyDto> OccupancyByDayOfWeek,
    IReadOnlyList<GlobalClubOccupancyDto> OccupancyByClub,
    IReadOnlyList<MonthOccupancyDto>  OccupancyEvolution
);

public sealed record GlobalClubOccupancyDto(Guid ClubId, string CourtName, double OccupancyRate);
