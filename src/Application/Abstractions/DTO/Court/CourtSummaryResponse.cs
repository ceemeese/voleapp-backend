namespace Application.Abstractions.DTO.Court;

public sealed record CourtSummaryResponse(
    Guid Id,
    string Name,
    CourtTypeResponse Type,
    decimal BasePrice,
    bool IsActive
);