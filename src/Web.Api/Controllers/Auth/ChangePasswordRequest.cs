namespace Web.Api.Controllers.Auth;

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);