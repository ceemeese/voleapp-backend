using FluentValidation;

namespace Application.Supports.Commands;

internal sealed class SendContactValidator : AbstractValidator<SendContact>
{
    public SendContactValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("Contact.NameRequired").WithMessage("El nombre es obligatorio ");
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode("Contact.EmailRequired").WithMessage("El correo es obligatorio ")
            .EmailAddress().WithErrorCode("Contact.EmailInvalid").WithMessage("Formato de email no válido");
        RuleFor(x => x.Message)
            .NotEmpty().WithErrorCode("Contact.MessageRequired").WithMessage("El mensaje es obligatorio");
    }
}