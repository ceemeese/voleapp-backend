using SharedKernel;

namespace Domain.Club.Entities;

public static class PricingConfigErrors
{
    public static readonly Error InvalidPercentage = Error.Validation(
        "PricingConfig.InvalidPercentage",
        "Los porcentajes deben estar entre 0 y 100");
    
    public static Error NotFound(Guid clubId) => Error.NotFound(
        "PricingConfig.NotFound",
        $"El club con id {clubId} no se encontró");
}