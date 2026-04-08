using System.Formats.Asn1;
using Domain.Common;
using Domain.Court.Entities;
using Domain.Court.Enum;
using SharedKernel;

namespace Domain.Court;

public sealed class Court : AggregateRoot<Guid>
{
    public Guid ClubId { get; private set; }
    public string Name { get; private set; }
    public CourtType Type { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<CourtEvent> _courtEvents = new();
    //expression bodied members => devolucion de propiedad de solo lectura(private readonly)
    public IReadOnlyCollection<CourtEvent> CourtEvents => _courtEvents.AsReadOnly();
    
    
    internal Court(Guid id, Guid clubId, string name, CourtType type, decimal basePrice, bool isActive)
    {
        ClubId = clubId;
        Name = name;
        Type = type;
        BasePrice = basePrice;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Court()
    {
    }
    
    public static Result<Court> Create(Guid clubId, string name, CourtType type, decimal basePrice, bool isActive)
    {
        if (basePrice <= 0)
        {
            return Result.Failure<Court>(CourtErrors.InvalidPrice);
        }
        
        var court = new Court(
            Guid.NewGuid(),
            clubId, 
            name, 
            type, 
            basePrice, 
            isActive);

        return Result.Success(court);
    }
    
    public Result UpdateProfile(string name, decimal basePrice)
    {
        if (basePrice <= 0)
        {
            return Result.Failure(CourtErrors.InvalidPrice);
        }

        Name = name;
        BasePrice = basePrice;

        return Result.Success();
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    
    public Result<CourtEvent> AddEvent(DateTime startTime, DateTime endTime, string eventName, string? description)
    {
        var validateEventResult =  ValidateEventConflict(null, startTime, endTime);
        if (validateEventResult.IsFailure)
        {
            return Result.Failure<CourtEvent>(validateEventResult.Error);
        }

        var eventResult = CourtEvent.Create(this.Id, startTime, endTime, eventName, description);
        if (eventResult.IsFailure)
        {
            return Result.Failure<CourtEvent>(eventResult.Error);
        }

        _courtEvents.Add(eventResult.Value);
        return Result.Success(eventResult.Value);
    }
    
    public Result<CourtEvent> UpdateEvent(int eventId, DateTime startTime, DateTime endTime, string eventName, string? description)
    {
        var existingEvent = _courtEvents.FirstOrDefault(e => e.Id == eventId);
        if (existingEvent is null)
        {
            return Result.Failure<CourtEvent>(CourtEventErrors.NotFound(eventId));
        }

        var validateEventResult =  ValidateEventConflict(eventId, startTime, endTime);
        if (validateEventResult.IsFailure)
        {
            return Result.Failure<CourtEvent>(validateEventResult.Error);
        }

        var eventResult = existingEvent.Update(startTime, endTime, eventName, description);
        if (eventResult.IsFailure)
        {
            return Result.Failure<CourtEvent>(eventResult.Error); 
        }
        
        return Result.Success(existingEvent);
    }

    public Result DeleteEvent(int eventId)
    {
        var existingEvent = _courtEvents.FirstOrDefault(e => e.Id == eventId);
        if (existingEvent is null)
        {
            return Result.Failure(CourtEventErrors.NotFound(eventId));
        }
        
        _courtEvents.Remove(existingEvent);
        return Result.Success();
    }
    
    private Result ValidateEventConflict(int? updatingEventId, DateTime startTime, DateTime endTime)
    {
        if (!IsActive)
            return Result.Failure(CourtErrors.NotActive);

        var hasConflict = _courtEvents.Any(e => (updatingEventId == null || e.Id != updatingEventId) && e.StartTime < endTime && e.EndTime > startTime);
        if (hasConflict)
        {
            return Result.Failure(CourtErrors.SlotOccupied);   
        }
        
        return Result.Success();
    }
}