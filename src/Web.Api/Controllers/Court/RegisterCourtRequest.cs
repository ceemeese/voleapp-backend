namespace Web.Api.Controllers.Court;

public sealed record RegisterCourtRequest(
    string Name,
    string Type,
    decimal BasePrice,
    bool IsActive
);