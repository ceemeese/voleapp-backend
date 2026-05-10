using Application.Abstractions.DTO.Weather;

namespace Application.Abstractions.Interfaces;

public interface IWeatherService
{
    Task<WeatherSummary?> GetWeatherForecastAsync(string city, string country, DateTime date);
}
