using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Reservation.Enum;
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
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public PriceBreakdown Price { get; private set; }
    
    internal Reservation(Guid userId, Guid clubId, Guid courtId, DateOnly date, TimeOnly startTime, TimeOnly endTime, PriceBreakdown price, string? notes = null)
    {
        UserId = userId;
        ClubId = clubId;
        CourtId = courtId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Price = price;
        Notes = notes;
        Status = Status.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Reservation()
    {
    }

    public static Result<Reservation> Create(Guid userId, Guid clubId, Guid courtId, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, PriceBreakdown price, string? notes)
    {
        var validationResult = ValidateReservationRules(date, startTime, endTime, price.TotalPrice);
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
            price,
            notes);

        return Result.Success(reservation);
    }

    public Result ChangeStatus(Status newStatus)
    {
        if (Status == newStatus)
        {
            return Result.Success();
        }

        if (Status is Status.Completed or Status.Failed or Status.Refunded or Status.Cancelled)
        {
            return Result.Failure(ReservationErrors.AlreadyFinalized);
        }

        if (Status is Status.Confirmed && newStatus is Status.Pending)
        {
            return Result.Failure(ReservationErrors.CannotMoveBackToPending);
        }
        
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is Status.Cancelled)
        {
            return Result.Failure(ReservationErrors.AlreadyFinalized);
        }
        
        if (Status is Status.Completed or Status.Failed or Status.Refunded or Status.Cancelled)
        {
            return Result.Failure(ReservationErrors.AlreadyFinalized);
        }
        
        Status = Status.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
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

        if (!IsValidDuration(startTime, endTime))
        {
            return Result.Failure(ReservationErrors.InvalidDuration);
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
    
    private static bool IsValidDuration(TimeOnly startTime, TimeOnly endTime)
    {
        var duration = (endTime.ToTimeSpan() - startTime.ToTimeSpan()).TotalMinutes;
        return duration == 60 || duration == 90;
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