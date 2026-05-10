using SharedKernel;

namespace Domain.Common.ValueObjects;

public record DateTimeRange
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int DurationMinutes {get; init;}

    public DateTimeRange(DateTime start, DateTime end, int durationMinutes)
    {
        Start = start;
        End = end;
        DurationMinutes = durationMinutes;
    }

    public static Result<DateTimeRange> Create(DateTime start, int  durationMinutes)
    {
        if (durationMinutes <= 0)
        {
            return Result.Failure<DateTimeRange>(new Error(
                "DateTimeRange.InvalidDuration", 
                "La duración debe ser mayor a 0 minutos", 
                ErrorType.Validation));
        }
        
        DateTime end = start.AddMinutes(durationMinutes);

        if (start < DateTime.UtcNow)
        {
            return Result.Failure<DateTimeRange>(new Error("DateTimeRange.PastDate", "No se pueden crear rangos en fechas pasadas", ErrorType.Validation));
        }
        
        return Result.Success(new DateTimeRange(start, end, durationMinutes));
    }
}