using Application.Abstractions.DTO.Reservation;

namespace Application.Abstractions.DTO.Court;

public sealed record CourtSummaryResponse(
    Guid Id,
    string Name,
    CourtTypeResponse Type,
    PriceResponse Price,
    bool IsActive
);