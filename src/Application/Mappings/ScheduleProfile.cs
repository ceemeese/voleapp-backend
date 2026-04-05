using Application.Abstractions.DTO.Schedule;
using AutoMapper;
using Domain.Club.Entities;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Application.Mappings;

internal sealed class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<DayOfWeek, DayOfWeekResponse>().ConvertUsing(src => new DayOfWeekResponse((int)src, src.ToString()));
        CreateMap<Schedule, ScheduleResponse>();
    }
}