namespace Application.Abstractions.DTO.Reservation;

public sealed record ReservationResponse(
    int Id,
    Guid UserId,
    Guid ClubId,
    Guid CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    StatusResponse Status,
    decimal TotalPrice,
    string? Notes,
    DateTime CreatedAt, 
    DateTime UpdatedAt
);