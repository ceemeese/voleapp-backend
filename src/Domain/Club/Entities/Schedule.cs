using Domain.Club.Enum;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Domain.Club.Entities;
using Domain.Common;

public sealed class Schedule : Entity<int>
{
    internal Schedule(Guid clubId, DayOfWeek dayOfWeek, TimeOnly openingTime, TimeOnly closingTime)
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
    
    //metodo para marcar club cerrado y otro para actualizar horas
}