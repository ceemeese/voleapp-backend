using FluentValidation;

namespace Application.Users.Commands.Update;

internal sealed class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public  UpdateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithErrorCode("User.UsernameRequired").WithMessage("El apodo es obligatorio");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithErrorCode("User.PhoneNumberRequired").WithMessage("El número de teléfono es obligatorio")
            .MaximumLength(9).WithErrorCode("User.PhoneNumberMaxLength").WithMessage("El número de teléfono no puede contener más de 9 dígitos");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithErrorCode("User.EmailRequired").WithMessage("El email es obligatorio")
            .EmailAddress().WithErrorCode("User.EmailInvalid").WithMessage("Formato de email no válido");
        
    }
}