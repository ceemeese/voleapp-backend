using Application.Abstractions.DTO;
using AutoMapper;
using Domain.Court;

namespace Application.Mappings;

internal sealed class CourtProfile : Profile
{
    public CourtProfile()
    {
        CreateMap<Court, CourtResponse>();
    }
}