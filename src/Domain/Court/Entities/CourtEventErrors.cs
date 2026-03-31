namespace Domain.Court.Entities;
using SharedKernel;

public static class CourtEventErrors
{
    public static readonly Error InvalidRange = Error.Validation(
        "CourtEvent.InvalidRange",
        "La fecha de fin debe ser posterior a la de inicio");

    public static readonly Error PastDate = Error.Validation(
        "CourtEvent.PastDate",
        "No se pueden crear eventos en fechas pasadas");
    
    public static Error NotFound(int courtEventId) => Error.NotFound(
        "CourtEvent.NotFound",
        $"No se ha encontrado el evento con ID '{courtEventId}'");
    
    public static readonly Error CrossDayNotAllowed = Error.NotFound(
        "CourtEvent.NotFound",
        "Sólo se permiten reservas de mismo día");
}
