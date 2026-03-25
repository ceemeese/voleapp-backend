using SharedKernel;

namespace Domain.Club;

public static class ClubErrors
{
    public static Error NotFound(Guid clubId) => Error.NotFound(
        "Club.NotFound",
        $"El club con id {clubId} no se encontró");
}