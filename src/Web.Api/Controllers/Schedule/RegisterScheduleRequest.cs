namespace Web.Api.Controllers.Schedule;

public sealed record RegisterScheduleRequest(
    string DayOfWeek,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime
    );