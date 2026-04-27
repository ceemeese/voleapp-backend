using Application.Abstractions.DTO.Reservation;
using AutoMapper;
using Domain.Reservation;
using Domain.Reservation.Enum;

namespace Application.Mappings;

internal sealed class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Status, StatusResponse>().ConvertUsing(src => new StatusResponse((int)src, src.ToString()));
        CreateMap<Reservation, ReservationResponse>();
    }
}