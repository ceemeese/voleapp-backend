namespace Application.Abstractions.DTO.CourtEvent;

public sealed record CourtEventResponse(
    int Id,
    Guid CourtId,
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description,
    DateTime CreatedAt
);