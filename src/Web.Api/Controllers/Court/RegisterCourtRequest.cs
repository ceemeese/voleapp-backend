namespace Web.Api.Controllers.Court;

public sealed record RegisterCourtRequest(
    string Name,
    string CourtType,
    decimal BasePrice,
    bool IsActive
);