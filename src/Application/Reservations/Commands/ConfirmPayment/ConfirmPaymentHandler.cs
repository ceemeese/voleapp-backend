using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Reservation;
using Domain.Reservation.Enum;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.ConfirmPayment;

internal sealed class ConfirmPaymentHandler : IRequestHandler<ConfirmPayment, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IStripeService _stripeService;

    public ConfirmPaymentHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork, IUserContext userContext, IStripeService stripeService)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _stripeService = stripeService;
    }

    public async Task<Result> Handle(ConfirmPayment request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
            return Result.Failure(ReservationErrors.NotFound(request.ReservationId));

        if (!_userContext.IsOwnerOrSuperadmin(reservation.UserId))
            return Result.Failure(UserErrors.Forbidden);

        var succeeded = await _stripeService.VerifySessionSucceededAsync(request.SessionId);
        if (!succeeded)
            return Result.Failure(ReservationErrors.PaymentNotSucceeded);

        var changeResult = reservation.ChangeStatus(Status.Confirmed);
        if (changeResult.IsFailure)
            return Result.Failure(changeResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
