using Application.Abstractions.DTO.PricingConfig;
using MediatR;
using SharedKernel;

namespace Application.PricingConfig.Queries.Get;

public sealed record GetPricingConfig(Guid ClubId) : IRequest<Result<PricingConfigResponse>>;