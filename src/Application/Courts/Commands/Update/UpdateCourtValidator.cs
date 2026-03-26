using FluentValidation;

namespace Application.Courts.Commands.Update;

internal sealed class UpdateCourtValidator : AbstractValidator<UpdateCourt>
{
    public UpdateCourtValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithErrorCode("Court.NameRequired").WithMessage("El nombre de la pista es obligatorio");
        
        RuleFor(c => c.BasePrice)
            .NotEmpty().WithErrorCode("Court.BasePrice").WithMessage("La pista debe tener un precio base")
            .GreaterThan(0).WithErrorCode("Court.BasePrice").WithMessage("La pista debe tener un precio base superior a 0");
    }
}