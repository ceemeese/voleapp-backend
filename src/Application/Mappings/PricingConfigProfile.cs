using Application.Abstractions.DTO.PricingConfig;
using AutoMapper;

namespace Application.Mappings;

internal sealed class PricingConfigProfile : Profile
{
    public PricingConfigProfile()
    {
        CreateMap<Domain.Club.Entities.PricingConfig, PricingConfigResponse>();
    }
}