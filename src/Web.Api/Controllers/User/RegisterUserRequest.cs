namespace Web.Api.Controllers.User;

public sealed record RegisterUserRequest(string Name, string LastName, string Username, string Email, string PhoneNumber, string Password);
