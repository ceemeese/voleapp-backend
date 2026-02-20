using FluentValidation;

namespace Application.Users.Commands.ChangePassword;

internal sealed class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPassword>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(c=> c.NewPassword)
            .NotEmpty().WithErrorCode("User.PasswordRequired").WithMessage("La contraseña es obligatoria")
            .MinimumLength(8).WithErrorCode("User.MinRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .MaximumLength(15).WithErrorCode("User.MaxRequired").WithMessage("La contraseña debe contener entre 8 y 15 carácteres")
            .Matches("[A-Z]").WithErrorCode("User.PasswordRequiredCapitalized").WithMessage("La contraseña debe contener una letra mayúscula")
            .Matches("[a-z]").WithErrorCode("User.PasswordRequiredLowerCase").WithMessage("La contraseña debe contener una letra minúscula")
            .Matches("[0-9]").WithErrorCode("User.PasswordRequiredDigit").WithMessage("La contraseña debe contener un dígito");

        RuleFor(c => c.OldPassword)
            .NotEmpty().WithErrorCode("User.PasswordRequired").WithMessage("La contraseña antigua no puede estar vacía");
    }
}