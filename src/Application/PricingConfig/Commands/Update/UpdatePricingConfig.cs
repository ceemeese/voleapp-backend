using Application.Abstractions.DTO.PricingConfig;
using MediatR;
using SharedKernel;

namespace Application.PricingConfig.Commands.Update;

public sealed record UpdatePricingConfig(
    Guid ClubId,
    decimal RainDiscountPercent,
    double WindThreshold,
    decimal WindDiscountPercent,
    double HeatThreshold,
    decimal HeatDiscountPercent,
    double ColdThreshold,
    decimal ColdDiscountPercent
) : IRequest<Result<PricingConfigResponse>>;