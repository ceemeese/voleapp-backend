namespace Web.Api.Controllers.CourtEvent;

public sealed record UpdateCourtEventRequest(
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description
    );