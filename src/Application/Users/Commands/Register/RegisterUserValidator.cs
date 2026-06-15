using FluentValidation;

namespace Application.Users.Commands.Register;

internal sealed class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("User.NameRequired").WithMessage("El nombre es obligatorio");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithErrorCode("User.LastNameRequired").WithMessage("El apellido es obligatorio");
        
        RuleFor(x => x.Username)
            .NotEmpty().WithErrorCode("User.UsernameRequired").WithMessage("El apodo es obligatorio");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithErrorCode("User.PhoneNumberRequired").WithMessage("El número de teléfono es obligatorio")
            .MaximumLength(9).WithErrorCode("User.PhoneNumberMaxLength").WithMessage("El número de teléfono no puede contener más de 9 dígitos");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithErrorCode("User.EmailRequired").WithMessage("El email es obligatorio")
            .EmailAddress().WithErrorCode("User.EmailInvalid").WithMessage("Formato de email no válido");

        RuleFor(user => user.Password)
            .NotEmpty().WithErrorCode("User.PasswordRequired").WithMessage("La contraseña es obligatoria")
            .MinimumLength(8).WithErrorCode("User.MinRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .MaximumLength(15).WithErrorCode("User.MaxRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .Matches("[A-Z]").WithErrorCode("User.PasswordRequiredCapitalized").WithMessage("La contraseña debe contener una letra mayúscula")
            .Matches("[a-z]").WithErrorCode("User.PasswordRequiredLowerCase").WithMessage("La contraseña debe contener una letra minúscula")
            .Matches("[0-9]").WithErrorCode("User.PasswordRequiredDigit").WithMessage("La contraseña debe contener un dígito");
    }
}