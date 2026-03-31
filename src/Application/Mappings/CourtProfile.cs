using Application.Abstractions.DTO.Court;
using Application.Abstractions.DTO.CourtEvent;
using AutoMapper;
using Domain.Court;
using Domain.Court.Entities;
using Domain.Court.Enum;

namespace Application.Mappings;

internal sealed class CourtProfile : Profile
{
    public CourtProfile()
    {
        CreateMap<CourtType, CourtTypeResponse>().ConvertUsing(src => new CourtTypeResponse((int)src, src.ToString()));
        CreateMap<CourtEvent, CourtEventResponse>();
        CreateMap<Court, CourtResponse>()
            .ForMember(dest => dest.CourtEvents, opt => opt.MapFrom(src => src.CourtEvents));
        CreateMap<Court, CourtSummaryResponse>();
    }
}