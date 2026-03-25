using Application.Abstractions.DTO;
using AutoMapper;
using Domain.Club;

namespace Application.Mappings;

internal sealed class ClubProfile : Profile
{
    public ClubProfile()
    {
        CreateMap<Club, ClubResponse>();
    }
}