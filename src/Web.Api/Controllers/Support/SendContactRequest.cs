namespace Web.Api.Controllers.Support;

public sealed record SendContactRequest(
    string Name,
    string Email,
    string Message
);