using Domain.Club.Enum;
using Domain.Common;

namespace Domain.Club.Entities;

public sealed class Court : Entity<int>
{
    internal Court(Guid clubId, string name, CourtType type, decimal basePrice, bool isActive)
    {
        ClubId = clubId;
        Name = name;
        Type = type;
        BasePrice = basePrice;
        IsActive = isActive;
    }
    
    private Court()
    {
    }
    
    public Guid ClubId { get; private set; }
    public string Name { get; private set; }
    public CourtType Type { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    private readonly List<CourtEvent> _courtsEvents = new();
    //expression bodied members =>
    public IReadOnlyCollection<CourtEvent> CourtsEvent => _courtsEvents.AsReadOnly();
    
    
    //evento anadir eventopista
}