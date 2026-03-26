namespace Application.Abstractions.DTO.Court;

public sealed record CourtEventResponse(
    Guid CourtId,
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description,
    DateTime CreatedAt
);