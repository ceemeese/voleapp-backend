namespace Application.Abstractions.DTO;

public sealed record ClubSummaryResponse(
    Guid Id,
    string Name,
    string Address,
    bool IsActive
);