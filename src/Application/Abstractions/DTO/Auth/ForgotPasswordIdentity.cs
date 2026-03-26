namespace Application.Abstractions.DTO.Auth;

public sealed record ForgotPasswordIdentity(string Token, string Email);