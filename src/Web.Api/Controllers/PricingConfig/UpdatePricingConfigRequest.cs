namespace Web.Api.Controllers.PricingConfig;

public sealed record UpdatePricingConfigRequest(
    decimal RainDiscountPercent,
    double WindThreshold,
    decimal WindDiscountPercent,
    double HeatThreshold,
    decimal HeatDiscountPercent,
    double ColdThreshold,
    decimal ColdDiscountPercent
);