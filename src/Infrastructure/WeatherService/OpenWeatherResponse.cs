using System.Text.Json.Serialization;

namespace Infrastructure.WeatherService;

internal sealed class OpenWeatherResponse
{
    [JsonPropertyName("list")]
    public List<WeatherInterval> List { get; set; } = new();
}

internal sealed class WeatherInterval
{
    //unix UTC
    [JsonPropertyName("dt")]
    public long Dt { get; set; }
    
    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDetail> Weather { get; set; } = new();

    [JsonPropertyName("pop")]
    public double Pop { get; set; }

    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; } = string.Empty;
}

internal sealed class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
}


internal sealed class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
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