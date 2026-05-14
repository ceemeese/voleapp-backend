using Domain.Common.ValueObjects;

namespace Application.Abstractions.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetWeatherForecastAsync(string city, string country, DateTime date);
}
