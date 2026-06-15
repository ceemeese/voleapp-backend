using Application.Abstractions.DTO.Court;
using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Common.Services;
using Domain.Common.ValueObjects;
using Domain.Court;
using Domain.Court.Service;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.Availability;

internal sealed class AvailabilityCourtsHandler : IRequestHandler<AvailabilityCourts, Result<List<CourtGroupedAvailabilityResponse>>>
{
    private readonly IAvailabilityService _availabilityService;
    private readonly IMapper _mapper;
    private readonly ICourtRepository _courtRepository;
    private readonly IClubRepository _clubRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPricingService _pricingService;
    private readonly IWeatherService _weatherService;

    public AvailabilityCourtsHandler(IAvailabilityService availabilityService, IMapper mapper, ICourtRepository courtRepository,  IClubRepository clubRepository,  IReservationRepository reservationRepository, IPricingService pricingService,  IWeatherService weatherService)
    {
        _availabilityService = availabilityService;
        _mapper = mapper;
        _courtRepository = courtRepository;
        _clubRepository = clubRepository;
        _reservationRepository = reservationRepository;
        _pricingService = pricingService;
        _weatherService = weatherService;
    }

    public async Task<Result<List<CourtGroupedAvailabilityResponse>>> Handle(AvailabilityCourts request, CancellationToken cancellationToken)
    {
        //clubs con horarios filtrados por ciudad
        var clubs = await _clubRepository.GetActiveClubsByCityWithSchedulesAsync(request.City, cancellationToken);
        if (!clubs.Any())
        {
            return Result.Success(new List<CourtGroupedAvailabilityResponse>());
        }
        
        var clubsId = clubs.Select(c => c.Id).ToList();
        var startOfDay = request.RequestDateTime.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        //pistas con eventos en el dia solicitado
        var courts = await _courtRepository.GetCourtsByClubIdWithFilterEventsAsync(clubsId, startOfDay, endOfDay, cancellationToken);
        if (!courts.Any())
        {
            return Result.Success(new List<CourtGroupedAvailabilityResponse>());
        }

        //reservas existentes
        var requestDateOnly = DateOnly.FromDateTime(request.RequestDateTime);
        var courtsId = courts.Select(c => c.Id).ToList();
        var existingReservations = await _reservationRepository.GetReservationsByCourtIdFilterDate(courtsId, requestDateOnly, cancellationToken);
        
        var availabilityResult = _availabilityService.GetAvailableCourts(
            request.RequestDateTime,
            request.DurationMinutes,
            request.City,
            clubs,
            courts,
            existingReservations
        );

        if (availabilityResult.IsFailure)
        {
            return Result.Failure<List<CourtGroupedAvailabilityResponse>>(availabilityResult.Error);
        }
        
        //pais del primer club
        var weatherData = await _weatherService.GetWeatherForecastAsync(request.City, clubs.First().Address.Country, request.RequestDateTime);
        var finalWeather = weatherData ?? WeatherData.Default;
        
        var availableCourts = availabilityResult.Value;
        
        var startTime = TimeOnly.FromDateTime(request.RequestDateTime);
        var endTime = TimeOnly.FromDateTime(request.RequestDateTime.AddMinutes(request.DurationMinutes));
        
        var response = BuildGroupedResponse(availableCourts, clubs, finalWeather, startTime, endTime);
        return Result.Success(response);
    }
    
    
    
    
    private List<CourtGroupedAvailabilityResponse> BuildGroupedResponse(
        List<Court> availableCourts,
        List<Club> clubs,
        WeatherData weather,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        return availableCourts
            .GroupBy(c => c.ClubId)
            .Select(group =>
            {
                var club = clubs.First(c => c.Id == group.Key);
                var courtSummaries = group.Select(court =>
                {
                    var pricingResult = _pricingService.CalculateTransactionPrice(
                        court.BasePrice, club.PricingConfig, weather, startTime, endTime);

                    return new CourtAvailabilityDetailResponse(
                        court.Id,
                        court.Name,
                        _mapper.Map<CourtTypeResponse>(court.Type),
                        _mapper.Map<PriceResponse>(pricingResult),
                        court.IsActive);
                }).ToList();

                return new CourtGroupedAvailabilityResponse(
                    club.Id, club.Name, club.Address.ToString(),
                    weather.IconUrl, courtSummaries);
            })
            .ToList();
    }
}