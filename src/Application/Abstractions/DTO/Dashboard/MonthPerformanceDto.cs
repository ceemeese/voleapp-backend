namespace Application.Abstractions.DTO.Dashboard;

public sealed record MonthPerformanceDto(
    string MonthName,
    int MonthNumber,
    decimal TotalRevenue,
    int TotalReservations
);