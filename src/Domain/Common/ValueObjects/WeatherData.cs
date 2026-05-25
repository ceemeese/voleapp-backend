namespace Domain.Common.ValueObjects;

public sealed record WeatherData(
    bool IsRaining,
    double Temperature,
    double WindSpeed,
    string Description,
    double RainProbability,
    string? IconCode)
{
    
    public string IconUrl => !string.IsNullOrWhiteSpace(IconCode)
        ? $"https://openweathermap.org/img/wn/{IconCode.Replace("n", "d")}@2x.png"
        : "https://openweathermap.org/img/wn/01d@2x.png";

    public static readonly WeatherData Default = new(
        IsRaining: false,
        Temperature: 20,
        WindSpeed: 0,
        Description: string.Empty,
        RainProbability: 0,
        IconCode: "01d"
    );
}
    
    
    
    
