namespace Application.Abstractions.DTO.Court;

public sealed record CourtResponse(
    Guid Id,
    Guid ClubId,
    string Name,
    CourtTypeResponse Type,
    decimal BasePrice,
    bool IsActive,
    DateTime CreatedAt,
    List<CourtEventResponse> CourtEvents
);