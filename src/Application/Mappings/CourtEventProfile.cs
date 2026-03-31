using Application.Abstractions.DTO.CourtEvent;
using AutoMapper;
using Domain.Court.Entities;

namespace Application.Mappings;

internal sealed class CourtEventProfile : Profile
{
    public CourtEventProfile()
    {
        CreateMap<CourtEvent, CourtEventResponse>();
    }
}