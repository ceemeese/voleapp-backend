using FluentValidation;

namespace Application.Clubs.Commands.Update;

internal sealed class UpdateClubValidator : AbstractValidator<UpdateClub>
{
    public UpdateClubValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithErrorCode("Club.NameRequired").WithMessage("El nombre es obligatorio");

        RuleFor(c => c.Cif)
            .NotEmpty().WithErrorCode("Club.CifRequired").WithMessage("El Cif es obligatorio")
            .MaximumLength(9).WithErrorCode("Club.CifMaxLength").WithMessage("El código de identificación no puede contener más de 9 dígitos");
        
        RuleFor(c => c.Street)
            .NotEmpty().WithErrorCode("Club.StreetRequired").WithMessage("La calle es obligatoria");
        
        RuleFor(c => c.City)
            .NotEmpty().WithErrorCode("Club.CityRequired").WithMessage("La ciudad es obligatoria");
        
        RuleFor(c => c.ZipCode)
            .NotEmpty().WithErrorCode("Club.ZipCodeRequired").WithMessage("El código postal es obligatorio")
            .MaximumLength(5).WithErrorCode("Club.ZipCodeMaxLength").WithMessage("El código postal no puede contener más de 5 dígitos");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithErrorCode("Club.PhoneNumberRequired").WithMessage("El número de teléfono es obligatorio")
            .MaximumLength(9).WithErrorCode("Club.PhoneNumberMaxLength").WithMessage("El número de teléfono no puede contener más de 9 dígitos");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithErrorCode("Club.EmailRequired").WithMessage("El email es obligatorio")
            .EmailAddress().WithErrorCode("Club.EmailInvalid").WithMessage("Formato de email no válido");
    }
}