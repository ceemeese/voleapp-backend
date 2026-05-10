using System.Text.Json.Serialization;

namespace Infrastructure.WeatherService;

internal sealed class OpenWeatherResponse
{
    [JsonPropertyName("daily")]
    public List<DailyForecast> Daily { get; set; } = new();
}

internal sealed class DailyForecast
{
    //unix UTC
    [JsonPropertyName("dt")]
    public long Dt { get; set; }
    
    [JsonPropertyName("temp")]
    public TemperatureData Temp { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDetail> Weather { get; set; } = new();
    
    [JsonPropertyName("pop")]
    public double PrecipitationProbability { get; set; }
    [JsonPropertyName("wind_speed")]
    public double WindSpeed { get; set; }
}

internal sealed class WeatherDetail
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

internal sealed class TemperatureData
{
    [JsonPropertyName("day")]
    public double Day { get; set; }
}