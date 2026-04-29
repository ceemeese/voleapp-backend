using Domain.Reservation.Enum;
using FluentValidation;

namespace Application.Reservations.Commands.UpdateStatus;

internal sealed class UpdateStatusReservationValidator : AbstractValidator<UpdateStatusReservation>
{
    public UpdateStatusReservationValidator()
    {
        RuleFor(r => r.NewStatus)
            .NotEmpty().WithErrorCode("Reservation.StatusRequired").WithMessage("El nuevo estado es obligatorio")
            .Must(BeAValidStatus).WithErrorCode("Reservation.InvalidType").WithMessage("El nuevo estado no es valido");
        RuleFor(r => r.Id)
            .NotEmpty().WithErrorCode("Reservation.ReservationIdRequired").WithMessage("El is de la reserva es obligatorio");
    }
    
    private static bool BeAValidStatus(int status)
    {
        return Enum.IsDefined(typeof(Status), status);
    }
}