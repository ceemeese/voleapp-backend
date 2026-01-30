using Domain.Common;
using Domain.Reservation.Enum;

namespace Domain.Reservation;

public class Reservation : AggregateRoot<int>
{
    internal Reservation(Guid userId, Guid clubId, int courtId, DateOnly date, TimeOnly startTime, TimeOnly endTime, decimal totalPrice, string? notes = null)
    {
        UserId = userId;
        ClubId = clubId;
        CourtId = courtId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        TotalPrice = totalPrice;
        Notes = notes;
        Status = Status.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Reservation()
    {
    }
    
    public Guid UserId { get; private set; }
    public Guid ClubId { get; private set; }
    public int CourtId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public Status Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
}