using Application.Abstractions.DTO.Schedule;
using AutoMapper;
using Domain.Club.Entities;

namespace Application.Mappings;

internal sealed class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<DayOfWeek, DayOfWeekResponse>().ConvertUsing(src => new DayOfWeekResponse((int)src, src.ToString()));
        CreateMap<Schedule, ScheduleResponse>();
    }
}