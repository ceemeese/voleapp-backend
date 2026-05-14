using Application.Abstractions.DTO.Reservation;
using AutoMapper;
using Domain.Common.ValueObjects;
using Domain.Reservation;
using Domain.Reservation.Enum;

namespace Application.Mappings;

internal sealed class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<PriceBreakdown, PriceResponse>()
            .ConstructUsing(src => new PriceResponse(src.BasePrice, src.TotalPrice, src.DiscountAmount, src.AppliedDiscountPercent, src.DiscountReason));
        CreateMap<Status, StatusResponse>()
            .ConvertUsing(src => new StatusResponse((int)src, src.ToString()));
        CreateMap<Reservation, ReservationResponse>();
    }
}