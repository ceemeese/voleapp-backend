using FluentValidation;

namespace Application.Analytics.GetAnalytics;

internal sealed class GetAnalyticsValidator : AbstractValidator<GetAnalytics>
{
    public GetAnalyticsValidator()
    {
        RuleFor(a => a.ClubId)
            .NotEmpty().WithErrorCode("Analytics.ClubRequired").WithMessage("El club es obligatorio");
        RuleFor(a => a.Year)
            .GreaterThan(2020).WithErrorCode("Analytics.YearRequired").WithMessage("El año es obligatorio y debe ser válido");
        RuleFor(a => a.Month)
            .InclusiveBetween(1,12).WithErrorCode("Analytics.MonthRequired").WithMessage("El mes es obligatorio");
    }
}