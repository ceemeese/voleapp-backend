namespace Web.Api.Controllers.User;

public sealed record UpdateUserRequest(string Username, string Email, string PhoneNumber);