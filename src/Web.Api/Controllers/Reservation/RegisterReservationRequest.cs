using Domain.Reservation.Enum;

namespace Web.Api.Controllers.Reservation;

public sealed record RegisterReservationRequest(
    Guid CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes
);