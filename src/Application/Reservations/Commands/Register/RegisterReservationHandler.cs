using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Common.ValueObjects;
using Domain.Court;
using Domain.Reservation;
using Domain.Reservation.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Reservations.Commands.Register;

internal sealed class RegisterReservationHandler : IRequestHandler<RegisterReservation, Result<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;
    private readonly IReservationService _reservationService;
    private readonly ICourtRepository _courtRepository;
    private readonly IWeatherService _weatherService;
    private readonly ILogger<RegisterReservationHandler> _logger;

    public RegisterReservationHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork, IMapper mapper,  IUserContext userContext,  IClubRepository clubRepository,  IReservationService reservationService, ICourtRepository courtRepository, IWeatherService weatherService,  ILogger<RegisterReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _clubRepository = clubRepository;
        _reservationService = reservationService;
        _courtRepository = courtRepository;
        _weatherService = weatherService;
        _logger = logger;
    }
    
    public async Task<Result<ReservationResponse>> Handle(RegisterReservation request,
        CancellationToken cancellationToken)
    {
        if (_userContext.UserId == Guid.Empty)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.Unauthenticated);
        }
        
        var court = await _courtRepository.GetCourtByIdAsync(request.CourtId, cancellationToken);
        if (court is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.CourtNotFound);
        }
        
        var club = await _clubRepository.GetClubWithMembersAsync(court.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.ClubNotFound(court.ClubId));
        }
        
        var existingReservations = await _reservationRepository.GetReservationsByCourtIdFilterDate([court.Id], request.Date, cancellationToken);
        
        DateTime reservationDateTime = request.Date.ToDateTime(request.StartTime);
        
        var weatherData = await _weatherService.GetWeatherForecastAsync(club.Address.City, club.Address.Country, reservationDateTime);
        var finalWeather = weatherData ?? WeatherData.Default;
        
        var reservationResult = _reservationService.BookCourt(
            _userContext.UserId, 
            court, 
            club,
            existingReservations,
            request.Date,
            request.StartTime, 
            request.EndTime, 
            request.Notes,
            finalWeather,
            club.PricingConfig
        );

        if (reservationResult.IsFailure)
        {
            return Result.Failure<ReservationResponse>(reservationResult.Error);
        }
        
        _logger.LogInformation("Reserva procesada para el usuario {UserId} en {City}. " +
                               "Resultado: {TotalPrice}€ (Descuento aplicado: {Discount}%). " +
                               "Condiciones: Temp {Temp}°C, Viento {Wind}km/h, Lluvia {Rain}%",
            _userContext.UserId,
            club.Address.City,
            reservationResult.Value.Price.TotalPrice,
            reservationResult.Value.Price.AppliedDiscountPercent,
            finalWeather.Temperature,
            finalWeather.WindSpeed,
            finalWeather.RainProbability);
        
        _reservationRepository.Add(reservationResult.Value);
        
        var ensureMemberResult = club.EnsureMembership(_userContext.UserId);
        if (ensureMemberResult.IsFailure)
        {
            return Result.Failure<ReservationResponse>(ensureMemberResult.Error);
        }
        
       await _unitOfWork.SaveChangesAsync(cancellationToken); 
       
       var reservationMapped = _mapper.Map<ReservationResponse>(reservationResult.Value);
       return Result.Success(reservationMapped);
    }
    
}