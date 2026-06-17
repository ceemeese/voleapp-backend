namespace Web.Api.Controllers.Auth;

public sealed record ConfirmEmailRequest(string Token, string Email);