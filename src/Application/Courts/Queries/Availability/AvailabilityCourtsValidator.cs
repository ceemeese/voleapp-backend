using FluentValidation;

namespace Application.Courts.Queries.Availability;

internal sealed class AvailabilityCourtsValidator : AbstractValidator<AvailabilityCourts>
{
    public AvailabilityCourtsValidator()
    {
        RuleFor(c => c.City)
            .NotEmpty().WithErrorCode("Availability.CityRequired").WithMessage("La ciudad es obligatoria");
        RuleFor(c => c.RequestDateTime)
            .NotEmpty().WithErrorCode("Availability.DateRequired").WithMessage("La fecha de búsqueda es obligatoria");
        RuleFor(c => c.DurationMinutes)
            .Must(duration => duration == 60 || duration == 90)
            .WithErrorCode("Availability.DurationMinutesRequired")
            .WithMessage("La duración debe ser 60 o 90 minutos");
    }
}