using SharedKernel;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Domain.Club.Entities;
using Domain.Common;

public sealed class Schedule : Entity<int>
{
    private Schedule(Guid clubId, DayOfWeek dayOfWeek, TimeOnly openingTime, TimeOnly closingTime)
    {
        ClubId = clubId;
        DayOfWeek = dayOfWeek;
        OpeningTime = openingTime;
        ClosingTime = closingTime;
        IsClosed = false;
    }

    private Schedule()
    {
        
    }
    
    public Guid ClubId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly OpeningTime { get; private set; }
    public TimeOnly ClosingTime { get; private set; }
    public bool IsClosed { get; private set; }


    internal static Result<Schedule> Create(Guid clubId, DayOfWeek dayOfWeek, TimeOnly openingTime, TimeOnly closingTime)
    {
        if (openingTime >= closingTime)
        {
            return Result.Failure<Schedule>(ScheduleErrors.InvalidHours);
        }
        
        var newSchedule = new Schedule(clubId, dayOfWeek, openingTime, closingTime);
        return Result.Success(newSchedule);
    }

    internal Result<Schedule> UpdateHours(TimeOnly openingTime, TimeOnly closingTime)
    {
        if (openingTime >= closingTime)
        {
            return Result.Failure<Schedule>(ScheduleErrors.InvalidHours);
        }
        
        OpeningTime = openingTime;
        ClosingTime = closingTime;
        IsClosed = false;

        return Result.Success<Schedule>(this);
    }
    
    internal void MaskAsClosed()
    {
        IsClosed = true;
    }
    
    internal void MaskAsOpen()
    {
        IsClosed = false;
    }
}