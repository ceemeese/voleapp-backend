using Domain.Common;

namespace Domain.Court.Entities;


public class CourtEvent : Entity<int>
{
    internal CourtEvent(Guid courtId, DateTime startTime, DateTime endTime, string eventName, string? description = null)
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
    
    public Guid CourtId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string EventName { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
}