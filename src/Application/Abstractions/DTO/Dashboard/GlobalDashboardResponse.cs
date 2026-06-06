namespace Application.Abstractions.DTO.Dashboard;

public sealed record GlobalDashboardResponse(
    int TotalActiveClubsToday,
    int TodayTotalGlobalReservations,
    double TodayGlobalOccupancyRate,
    int TotalClubsInPlatform,
    int TotalPlayersInPlatform,
    string TopClubToday
);