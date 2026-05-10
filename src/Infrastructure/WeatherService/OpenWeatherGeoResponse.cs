using System.Text.Json.Serialization;

namespace Infrastructure.WeatherService;

internal sealed class OpenWeatherGeoResponse
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}