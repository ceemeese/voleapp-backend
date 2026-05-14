using System.Net.Http.Json;
using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Domain.Common.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WeatherService;

internal sealed class WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger):IWeatherService
{
    public async Task<WeatherData?> GetWeatherForecastAsync(string city, string country, DateTime date)
    {
        var apiKey = configuration["WeatherApi:ApiKey"];
        var baseUrl = configuration["WeatherApi:BaseUrl"];
        var formattedDate = date.ToString("yyyy-MM-dd");
        
        var geoUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={apiKey}";
        var geoResponse = await httpClient.GetFromJsonAsync<List<OpenWeatherGeoResponse>>(geoUrl);
        var location = geoResponse?.FirstOrDefault();

        if (location == null) return null;

        var weatherUrl = $"{baseUrl}?lat={location.Lat}&lon={location.Lon}&appid={apiKey}&units=metric&lang=es&cnt=16";
        var weatherData = await httpClient.GetFromJsonAsync<OpenWeatherResponse>(weatherUrl);
        
        var dayBlocks = weatherData?.List?.Where(d => 
            DateTimeOffset.FromUnixTimeSeconds(d.Dt).UtcDateTime.Date == date.Date).ToList();

        if (dayBlocks == null || !dayBlocks.Any()) return null;
        
        //primer bloque del dia para datos generales
        var referenceBlock = dayBlocks.First();
        var weatherDetail = referenceBlock.Weather.FirstOrDefault();

        //en cualquier momento del dia
        var isRaining = dayBlocks.Any(b => b.Weather.Any(w => w.Main.Contains("Rain", StringComparison.OrdinalIgnoreCase)));
        var maxPop = dayBlocks.Max(b => b.Pop) * 100;
        
        logger.LogInformation("Consulta de clima realizada: {City}, {Country}. Resultado: {@WeatherData}", 
            city, country, new { 
                Temp = referenceBlock.Main.Temp, 
                Wind = referenceBlock.Wind.Speed,
                RainProb = maxPop,
                IsRaining = isRaining,
                Condition = weatherDetail?.Main,
                FullDesc = weatherDetail?.Description
            });
        
        return new WeatherData(
            IsRaining: isRaining,
            Temperature: referenceBlock.Main.Temp,
            WindSpeed: referenceBlock.Wind.Speed,
            Description: weatherDetail?.Description ?? "Sin descripción",
            RainProbability: maxPop,
            IconCode: weatherDetail?.Icon
        );
    }
}