namespace Application.Abstractions.DTO.PricingConfig;

public sealed record PricingConfigResponse(
    int Id,
    Guid ClubId,
    decimal RainDiscountPercent,
    double WindThreshold,
    decimal WindDiscountPercent,
    double HeatThreshold,
    decimal HeatDiscountPercent,
    double ColdThreshold,
    decimal ColdDiscountPercent
);