namespace Web.Api.Controllers.Schedule;

public sealed record UpdateScheduleRequest(
    TimeOnly OpeningTime,
    TimeOnly ClosingTime
);