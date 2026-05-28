namespace Application.Abstractions.DTO.Dashboard;

public sealed record ClubDashboardResponse(
    decimal TodayRevenue,
    decimal AverageTicketToday,
    int TodayTotalReservations,
    int TodayTotalClubEvents,
    double TodayGlobalOccupancyRate,
    int CancelledReservationsToday,
    string PeakHourToday
    );