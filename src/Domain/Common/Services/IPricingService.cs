using Domain.Club.Entities;
using Domain.Common.ValueObjects;

namespace Domain.Common.Services;

public interface IPricingService
{
    (decimal Price, decimal Discount) CalculatePricePerHour(decimal basePrice, PricingConfig config, WeatherData weather);
    PriceBreakdown CalculateTransactionPrice(decimal basePrice, PricingConfig config, WeatherData weather, TimeOnly start, TimeOnly end);
}