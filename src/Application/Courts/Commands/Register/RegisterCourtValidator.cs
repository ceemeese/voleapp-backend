using Domain.Court.Enum;
using FluentValidation;

namespace Application.Courts.Commands.Register;

internal sealed class RegisterCourtValidator : AbstractValidator<RegisterCourt>
{
    public RegisterCourtValidator()
    {
        RuleFor(c => c.ClubId)
            .NotEmpty().WithErrorCode("Court.ClubRequired").WithMessage("El club es obligatorio");

        RuleFor(c => c.Name)
            .NotEmpty().WithErrorCode("Court.NameRequired").WithMessage("El nombre de la pista es obligatorio");

        RuleFor(c => c.Type)
            .NotEmpty().WithErrorCode("Court.Type").WithMessage("El tipo de pista es obligatorio")
            .Must(BeAValidCourtType).WithErrorCode("Court.Type").WithMessage("El tipo de pista no es válido");

        RuleFor(c => c.BasePrice)
            .NotEmpty().WithErrorCode("Court.BasePrice").WithMessage("La pista debe tener un precio base")
            .GreaterThan(0).WithErrorCode("Court.BasePrice").WithMessage("La pista debe tener un precio base superior a 0");
    }

    private bool BeAValidCourtType(string type)
    {
        return Enum.TryParse<CourtType>(type, ignoreCase:true, out _);
    }
}