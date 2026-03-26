using SharedKernel;

namespace Domain.Court;

public static class CourtErrors
{
    public static Error NotFound(Guid courtId) => Error.NotFound(
        "Court.NotFound",
        $"La pista con id {courtId} no se encontró");
    
    public static Error InvalidType => Error.Validation(
        "Court.InvalidType",
        $"El tipo de pista no es válido");
    
    public static Error DuplicateName(string name) => Error.Conflict(
        "Court.DuplicateName",
        $"Ya existe una pista con el nombre '{name}' en este club");
    
    public static Error InvalidPrice => Error.Validation(
        "Court.InvalidPrice",
        $"El precio de la pista debe ser mayor que cero");
}