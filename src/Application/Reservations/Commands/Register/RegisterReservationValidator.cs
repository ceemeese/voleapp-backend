using FluentValidation;

namespace Application.Reservations.Commands.Register;

internal sealed class RegisterReservationValidator : AbstractValidator<RegisterReservation>
{
    public RegisterReservationValidator()
    {
        RuleFor(c => c.CourtId)
            .NotEmpty().WithErrorCode("Reservation.CourtRequired").WithMessage("La pista es obligatoria");
        RuleFor(c => c.Date)
            .NotEmpty().WithErrorCode("Reservation.DateRequired").WithMessage("La fecha es obligatoria");
        RuleFor(c => c.StartTime)
            .NotEmpty().WithErrorCode("Reservation.StartTimeRequired").WithMessage("La hora inicio es obligatoria");
        RuleFor(c => c.EndTime)
            .NotEmpty().WithErrorCode("Reservation.EndTimeRequired").WithMessage("La hora fin es obligatoria");
    }
}