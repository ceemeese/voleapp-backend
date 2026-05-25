using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetUserReservations;

internal sealed class GetUserReservationsHandler : IRequestHandler<GetUserReservations, Result<List<ReservationCompleteResponse>>>
{
    private readonly IUserContext _userContext;
    private readonly IReservationQueries _reservationQueries;

    public GetUserReservationsHandler(IUserContext userContext,  IReservationQueries reservationQueries)
    {
        _userContext = userContext;
        _reservationQueries = reservationQueries;
    }

    public async Task<Result<List<ReservationCompleteResponse>>> Handle(GetUserReservations request, CancellationToken cancellationToken)
    {
        if (request.UserId != _userContext.UserId && !_userContext.IsSuperAdmin)
        {
            return Result.Failure<List<ReservationCompleteResponse>>(ReservationErrors.Forbidden);
        }

        var reservations = await _reservationQueries.GetAllReservationsAsync(_userContext.UserId, null, request.StartDate, request.EndDate, cancellationToken);
        return Result.Success(reservations);
    }
}