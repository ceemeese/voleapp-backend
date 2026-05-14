namespace Domain.Common.ValueObjects;

public sealed record WeatherData(
    bool IsRaining,
    double Temperature,
    double WindSpeed,
    string Description,
    double RainProbability,
    string? IconCode)
{

    public static readonly WeatherData Default = new(
        IsRaining: false,
        Temperature: 20,
        WindSpeed: 0,
        Description: string.Empty,
        RainProbability: 0,
        IconCode: null
    );
}
    
    
    
    
