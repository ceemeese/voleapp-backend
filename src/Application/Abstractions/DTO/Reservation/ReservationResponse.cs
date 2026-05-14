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
    PriceResponse Price,
    string? Notes,
    DateTime CreatedAt, 
    DateTime UpdatedAt
);


public sealed record PriceResponse
(
    decimal BasePrice,
    decimal TotalPrice,
    decimal DiscountAmount,
    double AppliedDiscountPercent,
    string? DiscountReason
);