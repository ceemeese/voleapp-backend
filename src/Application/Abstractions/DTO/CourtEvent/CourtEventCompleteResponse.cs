namespace Application.Abstractions.DTO.CourtEvent;

public sealed record CourtEventCompleteResponse(
    int Id,
    Guid CourtId,
    string CourtName,
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description,
    DateTime CreatedAt
);