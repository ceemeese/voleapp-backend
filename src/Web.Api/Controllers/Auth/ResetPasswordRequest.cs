namespace Web.Api.Controllers.Auth;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);