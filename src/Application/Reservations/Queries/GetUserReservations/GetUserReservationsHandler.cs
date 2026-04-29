using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetUserReservations;

internal sealed class GetUserReservationsHandler : IRequestHandler<GetUserReservations, Result<List<ReservationResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserContext _userContext;

    public GetUserReservationsHandler(IMapper mapper, IReservationRepository reservationRepository, IUserContext userContext)
    {
        _mapper = mapper;
        _reservationRepository = reservationRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<ReservationResponse>>> Handle(GetUserReservations request, CancellationToken cancellationToken)
    {
        if (request.UserId != _userContext.UserId && !_userContext.IsSuperAdmin)
        {
            return Result.Failure<List<ReservationResponse>>(ReservationErrors.Forbidden);
        }

        var reservations = await _reservationRepository.GetAllReservationsAsync(_userContext.UserId, null, request.StartDate, request.EndDate, cancellationToken);
        var reservationsMapped = _mapper.Map<List<ReservationResponse>>(reservations);
        return Result.Success(reservationsMapped);
    }
}