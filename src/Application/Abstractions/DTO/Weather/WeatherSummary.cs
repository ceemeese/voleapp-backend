namespace Application.Abstractions.DTO.Weather;

public sealed record WeatherSummary(
    bool IsRaining,
    double Temperature,
    double WindSpeed,
    string Description,
    double RainProbability,
    string? IconCode
);