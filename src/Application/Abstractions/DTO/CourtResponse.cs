using Domain.Club.Enum;

namespace Application.Abstractions.DTO;

public sealed record CourtResponse(
    Guid Id,
    Guid ClubId,
    string Name,
    string Type,
    decimal BasePrice,
    bool IsActive,
    DateTime CreatedAt
);