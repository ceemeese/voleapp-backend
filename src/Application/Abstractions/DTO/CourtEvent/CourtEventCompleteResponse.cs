namespace Application.Abstractions.DTO.CourtEvent;

public sealed record CourtEventCompleteResponse(
    int Id,
    Guid CourtId,
    string CourtName,
    DateTime StartDate,
    DateTime EndDate,
    string EventName,
    string? Description,
    DateTime CreatedAt
);