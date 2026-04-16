using Application.Abstractions.DTO.Club;
using AutoMapper;
using Domain.Club;
using Domain.Common.ValueObjects;

namespace Application.Mappings;

internal sealed class ClubProfile : Profile
{
    public ClubProfile()
    {
        CreateMap<Address, AddressResponse>()
            .ConstructUsing(src => new AddressResponse(src.Street, src.City, src.ZipCode, src.Country));
        CreateMap<Club, ClubResponse>();
        CreateMap<Club, ClubSummaryResponse>();
    }
}