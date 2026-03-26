namespace Application.Abstractions.DTO.Auth;

public sealed record ForgotResponse(string Token, string Email);