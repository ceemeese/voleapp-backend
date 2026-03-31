namespace Web.Api.Controllers.CourtEvent;

public sealed record RegisterCourtEventRequest(
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description
);