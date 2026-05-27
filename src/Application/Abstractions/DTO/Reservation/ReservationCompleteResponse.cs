namespace Application.Abstractions.DTO.Reservation;

public record ReservationCompleteResponse(
    int Id,
    Guid UserId,
    string Username,
    string Email,
    Guid ClubId,
    string ClubName,
    Guid CourtId,
    string CourtName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    StatusResponse Status,
    PriceResponse Price,
    string? Notes,
    DateTime CreatedAt, 
    DateTime UpdatedAt
);