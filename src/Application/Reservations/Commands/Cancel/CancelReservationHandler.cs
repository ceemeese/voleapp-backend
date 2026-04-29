using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.Cancel;

internal sealed class CancelReservationHandler : IRequestHandler<CancelReservation, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CancelReservationHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(CancelReservation request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound(request.Id));
        }

        if (reservation.UserId != _userContext.UserId && !_userContext.IsAnyAdmin())
        {
            return Result.Failure(ReservationErrors.Forbidden);
        }

        var reservationResult = reservation.Cancel();
        if (reservationResult.IsFailure)
        {
            return Result.Failure(reservationResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}