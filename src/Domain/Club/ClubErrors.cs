using SharedKernel;

namespace Domain.Club;

public static class ClubErrors
{
    public static Error NotFound(Guid clubId) => Error.NotFound(
        "Club.NotFound",
        $"El club con id {clubId} no se encontró");
    
    public static readonly Error Forbidden = Error.Forbidden(
        "Club.Forbidden",
        "Usuario sin permisos");
}