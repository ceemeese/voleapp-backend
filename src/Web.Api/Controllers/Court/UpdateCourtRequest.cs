namespace Web.Api.Controllers.Court;

public record UpdateCourtRequest(
    string Name,
    decimal BasePrice
);