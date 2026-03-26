using Domain.Common;
using Domain.Court.Entities;
using Domain.Court.Enum;
using SharedKernel;

namespace Domain.Court;

public sealed class Court : AggregateRoot<Guid>
{
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
    
    public Guid ClubId { get; private set; }
    public string Name { get; private set; }
    public CourtType Type { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<CourtEvent> _courtEvents = new();
    //expression bodied members => devolucion de propiedad de solo lectura(private readonly)
    public IReadOnlyCollection<CourtEvent> CourtEvents => _courtEvents.AsReadOnly();
    
    
    //evento anadir eventopista
}