using SharedKernel;

namespace Domain.Club.Entities;

public static class ScheduleErrors
{
    public static readonly Error InvalidHours = Error.Validation(
        "Schedule.InvalidHours",
        "La fecha de apertura no puede ser posterior a la de cierre");
    
    public static readonly Error InvalidType = Error.Validation(
        "Schedule.InvalidType",
        $"El día de la semana no es válido");
}