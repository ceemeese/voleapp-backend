using FluentValidation;

namespace Application.Analytics.GetOccupancy;

internal sealed class GetOccupancyValidator : AbstractValidator<GetOccupancy>
{
    public GetOccupancyValidator()
    {
        RuleFor(a => a.ClubId)
            .NotEmpty().WithErrorCode("Occupancy.ClubRequired").WithMessage("El club es obligatorio");
        RuleFor(a => a.Year)
            .GreaterThan(2020).WithErrorCode("Occupancy.YearRequired").WithMessage("El año es obligatorio y debe ser válido");
        RuleFor(a => a.Month)
            .InclusiveBetween(1,12).WithErrorCode("Occupancy.MonthRequired").WithMessage("El mes es obligatorio");
    }
}