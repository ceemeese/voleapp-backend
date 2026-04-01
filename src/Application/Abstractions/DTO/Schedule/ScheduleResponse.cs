namespace Application.Abstractions.DTO.Schedule;

public sealed record ScheduleResponse(
    int Id,
    Guid ClubId,
    DayOfWeekResponse DayOfWeek,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime,
    bool IsClosed
);