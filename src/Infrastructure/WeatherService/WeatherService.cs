using System.Net.Http.Json;
using Application.Abstractions.DTO.Weather;
using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.WeatherService;

internal sealed class WeatherService(HttpClient httpClient, IConfiguration configuration):IWeatherService
{
    public async Task<WeatherSummary?> GetWeatherForecastAsync(string city, string country, DateTime date)
    {
        var apiKey = configuration["WeatherApi:ApiKey"];
        var baseUrl = configuration["WeatherApi:BaseUrl"];
        var formattedDate = date.ToString("yyyy-MM-dd");

        var geoUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={apiKey}";
        var geoResponse = await httpClient.GetFromJsonAsync<List<OpenWeatherGeoResponse>>(geoUrl);
        var location = geoResponse?.FirstOrDefault();

        if (location == null) return null;

        var weatherUrl = $"{baseUrl}?lat={location.Lat}&lon={location.Lon}&exclude=minutely,hourly&appid={apiKey}&units=metric";
        var weatherData = await httpClient.GetFromJsonAsync<OpenWeatherResponse>(weatherUrl);
        
        var dayForecast = weatherData?.Daily?.FirstOrDefault(d =>
            DateTimeOffset.FromUnixTimeSeconds(d.Dt).Date == date.Date);

        if (dayForecast == null) return null;
        
        var weatherDetail = dayForecast.Weather.FirstOrDefault();

        bool isRaining = dayForecast.Weather.Any(w => w.Main.Contains("Rain", StringComparison.OrdinalIgnoreCase));

        return new WeatherSummary(
            IsRaining: isRaining,
            Temperature: dayForecast.Temp.Day,
            WindSpeed: dayForecast.WindSpeed,
            Description: weatherDetail?.Description ?? "Sin descripción",
            RainProbability: dayForecast.PrecipitationProbability,
            IconCode: weatherDetail?.Icon
        );
    }
}