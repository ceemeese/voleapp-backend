using Application.Abstractions.Interfaces;
using Domain.Reservation;
using Domain.Reservation.Enum;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.UpdateStatus;

internal sealed class UpdateStatusReservationHandler : IRequestHandler<UpdateStatusReservation, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStatusReservationHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStatusReservation request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetReservationById(request.Id, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound(request.Id));
        }
        
        var reservationResult = reservation.ChangeStatus((Status)request.NewStatus);
        if (reservationResult.IsFailure)
        {
            return Result.Failure(reservationResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}