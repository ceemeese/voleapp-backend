namespace Application.Abstractions.DTO.Court;

public sealed record CourtGroupedAvailabilityResponse(
    Guid ClubId, 
    string ClubName,
    string Address, 
    string WeatherIcon,
    List<CourtSummaryResponse> AvailableCourts
);