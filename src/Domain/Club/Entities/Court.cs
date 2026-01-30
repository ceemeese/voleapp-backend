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
        CreatedAt = DateTime.UtcNow;
    }
    
    private Court()
    {
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