namespace Application.Abstractions.DTO.Club;

public sealed record ClubSummaryResponse(
    Guid Id,
    string Name,
    AddressResponse Address,
    bool IsActive
);