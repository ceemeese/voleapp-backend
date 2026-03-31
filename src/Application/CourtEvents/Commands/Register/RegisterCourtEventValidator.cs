using FluentValidation;

namespace Application.CourtEvents.Commands.Register;

internal sealed class RegisterCourtEventValidator : AbstractValidator<RegisterCourtEvent>
{
    public RegisterCourtEventValidator()
    {
        RuleFor(c => c.CourtId)
            .NotEmpty().WithErrorCode("CourtEvent.CourtIdRequired").WithMessage("La pista es obligatoria");
        
        RuleFor(c => c.EventName)
            .NotEmpty().WithErrorCode("CourtEvent.NameRequired").WithMessage("El nombre del evento es obligatorio");
        
        RuleFor(c => c.StartTime)
            .NotEmpty().WithErrorCode("CourtEvent.StartTimeRequired").WithMessage("La fecha inicio es obligatoria");
        
        RuleFor(c => c.EndTime)
            .NotEmpty().WithErrorCode("CourtEvent.EndTimeRequired").WithMessage("La fecha fin es obligatoria");
        
    }
}