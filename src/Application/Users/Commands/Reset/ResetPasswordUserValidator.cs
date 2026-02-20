using FluentValidation;

namespace Application.Users.Commands.Reset;

internal sealed class ResetPasswordUserValidator : AbstractValidator<ResetPasswordUser>
{
    public ResetPasswordUserValidator()
    {
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithErrorCode("User.PasswordRequired").WithMessage("La contraseña es obligatoria")
            .MinimumLength(8).WithErrorCode("User.MinRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .MaximumLength(15).WithErrorCode("User.MaxRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .Matches("[A-Z]").WithErrorCode("User.PasswordRequiredCapitalized").WithMessage("La contraseña debe contener una letra mayúscula")
            .Matches("[a-z]").WithErrorCode("User.PasswordRequiredLowerCase").WithMessage("La contraseña debe contener una letra minúscula")
            .Matches("[0-9]").WithErrorCode("User.PasswordRequiredDigit").WithMessage("La contraseña debe contener un dígito");
        
        RuleFor(r => r.Email)
            .NotEmpty().WithErrorCode("User.EmailRequired").WithMessage("El email es obligatorio")
            .EmailAddress().WithErrorCode("User.EmailInvalid").WithMessage("Formato de email no válido");

        RuleFor(r => r.Token)
            .NotEmpty().WithErrorCode("User.TokenRequired").WithMessage("El token es obligatorio");
    }
}