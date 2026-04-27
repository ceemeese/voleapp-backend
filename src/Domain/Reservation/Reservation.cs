using Domain.Common;
using Domain.Reservation.Enum;
using Domain.Reservation.Events;
using SharedKernel;

namespace Domain.Reservation;

public class Reservation : AggregateRoot<int>
{
    public Guid UserId { get; private set; }
    public Guid ClubId { get; private set; }
    public Guid CourtId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public Status Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    internal Reservation(Guid userId, Guid clubId, Guid courtId, DateOnly date, TimeOnly startTime, TimeOnly endTime, decimal totalPrice, string? notes = null)
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

    public static Result<Reservation> Create(Guid userId, Guid clubId, Guid courtId, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, decimal totalPrice, string? notes)
    {
        var validationResult = ValidateReservationRules(date, startTime, endTime, totalPrice);
        if (validationResult.IsFailure)
        {
            return Result.Failure<Reservation>(validationResult.Error);
        }

        var reservation = new Reservation(
            userId,
            clubId,
            courtId,
            date,
            startTime,
            endTime,
            totalPrice,
            notes);

        return Result.Success(reservation);
    }

    private static Result ValidateReservationRules(DateOnly date, TimeOnly startTime, TimeOnly endTime, decimal totalPrice)
    {
        if (IsPastDate(date))
        {
            return Result.Failure(ReservationErrors.PastDate);
        }

        if (IsInvalidTimeRange(startTime, endTime))
        {
            return Result.Failure(ReservationErrors.InvalidHours);
        }

        if (IsPastStartTime(date, startTime))
        {
            return Result.Failure(ReservationErrors.PastStartTime);
        }

        if (IsTooFarInFuture(date))
        {
            return Result.Failure(ReservationErrors.TooFarInFuture);
        }

        if (totalPrice < 0)
        {
            return Result.Failure(ReservationErrors.InvalidTotalPrice);
        }
        
        return Result.Success();
    }
    
    private static bool IsPastDate(DateOnly date)
        => date < DateOnly.FromDateTime(DateTime.UtcNow);
    
    private static bool IsPastStartTime(DateOnly date, TimeOnly startTime)
        => date == DateOnly.FromDateTime(DateTime.UtcNow) && startTime < TimeOnly.FromDateTime(DateTime.UtcNow);
    
    private static bool IsTooFarInFuture(DateOnly date)
        => date > DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1);

    private static bool IsInvalidTimeRange(TimeOnly startTime, TimeOnly endTime)
        => startTime >= endTime;
}