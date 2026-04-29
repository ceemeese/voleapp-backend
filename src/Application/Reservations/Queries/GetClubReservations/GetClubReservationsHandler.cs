using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetClubReservations;

internal sealed class GetClubReservationsHandler:IRequestHandler<GetClubReservations, Result<List<ReservationResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetClubReservationsHandler(IMapper mapper, IReservationRepository reservationRepository,  IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _mapper = mapper;
        _reservationRepository = reservationRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<List<ReservationResponse>>> Handle(GetClubReservations request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<List<ReservationResponse>>(ReservationErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<List<ReservationResponse>>(ReservationErrors.Forbidden);
            }
        }

        var reservations = await _reservationRepository.GetAllReservationsAsync(null, request.ClubId, request.StartDate, request.EndDate, cancellationToken);

        var reservationsMapped = _mapper.Map<List<ReservationResponse>>(reservations);
        return Result.Success(reservationsMapped);
    }
}