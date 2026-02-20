using FluentValidation;

namespace Application.Users.Commands.Forgot;

internal sealed class ForgotPasswordUserValidator : AbstractValidator<ForgotPasswordUser>
{
    public ForgotPasswordUserValidator()
    {
        RuleFor(f => f.Email)
            .NotEmpty().WithErrorCode("User.EmailRequired").WithMessage("El email no puede estar vacio");
    }
}