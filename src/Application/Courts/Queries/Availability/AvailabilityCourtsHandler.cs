using Application.Abstractions.DTO.Court;
using AutoMapper;
using Domain.Club;
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

    public AvailabilityCourtsHandler(IAvailabilityService availabilityService, IMapper mapper, ICourtRepository courtRepository,  IClubRepository clubRepository,  IReservationRepository reservationRepository)
    {
        _availabilityService = availabilityService;
        _mapper = mapper;
        _courtRepository = courtRepository;
        _clubRepository = clubRepository;
        _reservationRepository = reservationRepository;
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
        
        var availableCourts = availabilityResult.Value;
        var groupedResponse = availableCourts
            .GroupBy(c => c.ClubId)
            .Select(group => 
            {
                var club = clubs.First(c => c.Id == group.Key);
                
                return new CourtGroupedAvailabilityResponse(
                    club.Id,
                    club.Name,
                    club.Address.ToString(),
                    _mapper.Map<List<CourtSummaryResponse>>(group.ToList())
                );
            })
            .ToList();
        
        return Result.Success(groupedResponse);
    }
}