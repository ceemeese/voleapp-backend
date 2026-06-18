using FluentValidation;

namespace Application.PricingConfig.Commands.Update;

internal sealed class UpdatePricingConfigValidator : AbstractValidator<UpdatePricingConfig>
{
    public UpdatePricingConfigValidator()
    {
        RuleFor(p => p.ClubId)
            .NotEmpty().WithErrorCode("PricingConfig.ClubRequired").WithMessage("El club es obligatorio");
        RuleFor(p => p.RainDiscountPercent)
            .InclusiveBetween(0, 100)
            .WithErrorCode("PricingConfig.InvalidRainDiscount")
            .WithMessage("El descuento por lluvia debe estar entre 0 y 100");
        RuleFor(p => p.WindDiscountPercent)
            .InclusiveBetween(0, 100)
            .WithErrorCode("PricingConfig.InvalidWindDiscount")
            .WithMessage("El descuento por viento debe estar entre 0 y 100");
        RuleFor(p => p.HeatDiscountPercent)
            .InclusiveBetween(0, 100)
            .WithErrorCode("PricingConfig.InvalidHeatDiscount")
            .WithMessage("El descuento por calor debe estar entre 0 y 100");
        RuleFor(p => p.ColdDiscountPercent)
            .InclusiveBetween(0, 100)
            .WithErrorCode("PricingConfig.InvalidColdDiscount")
            .WithMessage("El descuento por frío debe estar entre 0 y 100");
        
        RuleFor(p => p.WindThreshold)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("PricingConfig.InvalidWindThreshold")
            .WithMessage("El umbral de viento no puede ser negativo");
        RuleFor(p => p.HeatThreshold)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("PricingConfig.InvalidHeatThreshold")
            .WithMessage("El umbral de calor no puede ser negativo");
        RuleFor(p => p.ColdThreshold)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("PricingConfig.InvalidColdThreshold")
            .WithMessage("El umbral de frío no puede ser negativo");
    }
}