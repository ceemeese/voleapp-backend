using FluentValidation;

namespace Application.Analytics.GetGlobalAnalytics;

internal sealed class GetGlobalAnalyticsValidator : AbstractValidator<GetGlobalAnalytics>
{
    public GetGlobalAnalyticsValidator()
    {
        RuleFor(a => a.Year)
            .GreaterThan(2020).WithErrorCode("Analytics.YearRequired").WithMessage("El año es obligatorio y debe ser válido");
        RuleFor(a => a.Month)
            .InclusiveBetween(1, 12).WithErrorCode("Analytics.MonthRequired").WithMessage("El mes es obligatorio");
    }
}
