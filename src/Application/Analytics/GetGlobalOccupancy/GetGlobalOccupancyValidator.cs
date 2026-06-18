using FluentValidation;

namespace Application.Analytics.GetGlobalOccupancy;

internal sealed class GetGlobalOccupancyValidator : AbstractValidator<GetGlobalOccupancy>
{
    public GetGlobalOccupancyValidator()
    {
        RuleFor(a => a.Year)
            .GreaterThan(2020).WithErrorCode("Occupancy.YearRequired").WithMessage("El año es obligatorio y debe ser válido");
        RuleFor(a => a.Month)
            .InclusiveBetween(1, 12).WithErrorCode("Occupancy.MonthRequired").WithMessage("El mes es obligatorio");
    }
}
