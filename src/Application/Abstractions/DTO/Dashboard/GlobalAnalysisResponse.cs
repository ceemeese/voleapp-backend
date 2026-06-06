namespace Application.Abstractions.DTO.Dashboard;

public sealed record GlobalAnalysisResponse(
    decimal TotalRevenuePeriod,
    int TotalClubsPeriod,
    int TotalReservationsPeriod,
    int TotalNewPlayersCount,
    double AverageReservationsPerClub,
    List<GlobalMonthPerformanceDto> MonthlyEvolution
);

public sealed record GlobalMonthPerformanceDto(
    string Month,
    int MonthNumber,
    decimal TotalRevenue,
    int TotalReservations,
    int TotalNewClubs,
    int TotalNewPlayers
);