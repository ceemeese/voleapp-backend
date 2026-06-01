namespace Application.Abstractions.DTO.Dashboard;

public sealed record AnalysisResponse(
    decimal TotalRevenuePeriod,
    decimal AverageRevenuePeriod,
    int TotalReservationsPeriod,
    int TotalNewUsersCount,
    List<MonthPerformanceDto> EvolutionDate
    );