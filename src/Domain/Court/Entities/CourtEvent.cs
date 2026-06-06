using Domain.Common;
using SharedKernel;

namespace Domain.Court.Entities;


public class CourtEvent : Entity<int>
{
    public Guid CourtId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string EventName { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    
    private CourtEvent(Guid courtId, DateTime startTime, DateTime endTime, string eventName, string? description = null)
    {
        CourtId = courtId;
        StartTime = startTime;
        EndTime = endTime;
        EventName = eventName;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    private CourtEvent()
    {
    }

    
    internal static Result<CourtEvent> Create(Guid courtId, DateTime startTime, DateTime endTime, string eventName, string? description)
    {
        var validateEventDateResult = ValidateEventDate(startTime, endTime);
        if (validateEventDateResult.IsFailure)
        {
            return Result.Failure<CourtEvent>(validateEventDateResult.Error);
        }
        
        var courtEvent = new CourtEvent(courtId, startTime, endTime, eventName, description);
        return Result.Success<CourtEvent>(courtEvent);
    }
    
    internal Result Update(DateTime startTime, DateTime endTime, string eventName, string? description)
    {
        var validateEventDateResult = ValidateEventDate(startTime, endTime);
        if (validateEventDateResult.IsFailure)
        {
            return Result.Failure(validateEventDateResult.Error);
        }
        
        StartTime = startTime;
        EndTime = endTime;
        EventName = eventName;
        Description = description;
        
        return Result.Success();
    }

    private static Result ValidateEventDate(DateTime startTime, DateTime endTime)
    {
        if (startTime.Date != endTime.Date)
        {
            return Result.Failure(CourtEventErrors.CrossDayNotAllowed);
        }
        
        if (startTime >= endTime)
        {
            return Result.Failure(CourtEventErrors.InvalidRange);
        }

        if (startTime < DateTime.UtcNow)
        {
            return Result.Failure(CourtEventErrors.PastDate);
        }
        return Result.Success();
    }
    
}