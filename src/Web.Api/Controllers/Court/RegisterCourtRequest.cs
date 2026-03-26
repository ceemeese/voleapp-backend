namespace Web.Api.Controllers.Court;

public sealed record RegisterCourtRequest(
    Guid ClubId, 
    string Name,
    string CourtType,
    decimal BasePrice,
    bool IsActive
);