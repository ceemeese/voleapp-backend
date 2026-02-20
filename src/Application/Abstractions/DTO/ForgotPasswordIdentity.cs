namespace Application.Abstractions.DTO;

public sealed record ForgotPasswordIdentity(string Token, string Email);