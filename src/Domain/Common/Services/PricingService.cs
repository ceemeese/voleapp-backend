using Domain.Club.Entities;
using Domain.Common.ValueObjects;

namespace Domain.Common.Services;

public sealed class PricingService : IPricingService
{
    public PriceBreakdown CalculateTransactionPrice(decimal basePrice, PricingConfig config, WeatherData weather, TimeOnly start, TimeOnly end)
    {
        var durationInHours = (decimal)(end.ToTimeSpan() - start.ToTimeSpan()).TotalHours;

        var (pricePerHourWithDiscount, discountPercent) = CalculatePricePerHour(basePrice, config, weather);
        
        var totalBasePrice = basePrice * durationInHours;
        var finalPrice = pricePerHourWithDiscount * durationInHours;

        return new PriceBreakdown(
            BasePrice: totalBasePrice,
            TotalPrice: finalPrice,
            DiscountAmount: totalBasePrice - finalPrice,
            AppliedDiscountPercent: (double)discountPercent,
            DiscountReason: discountPercent > 0 ? "Descuento por condiciones climatológicas" : "Precio estándar"
        );
    }

    public (decimal Price, decimal Discount) CalculatePricePerHour(decimal basePrice, PricingConfig config, WeatherData weather)
    {
        decimal highestDiscount = 0;

        if (weather.Temperature >= config.HeatThreshold)
        {
            highestDiscount = Math.Max(highestDiscount, config.HeatDiscountPercent);
        }
        
        if (weather.Temperature <= config.ColdThreshold)
        {
            highestDiscount = Math.Max(highestDiscount, config.ColdDiscountPercent);
        }
        
        if (weather.WindSpeed >= config.WindThreshold)
        {
            highestDiscount = Math.Max(highestDiscount, config.WindDiscountPercent);
        }
        
        if (weather.RainProbability >= 10)
        {
            highestDiscount = Math.Max(highestDiscount, config.RainDiscountPercent);
        }

        var finalPrice = basePrice * (1 - highestDiscount / 100);
        
        return (finalPrice, highestDiscount);
    }
}