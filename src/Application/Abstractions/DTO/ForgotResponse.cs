namespace Application.Abstractions.DTO;

public sealed record ForgotResponse(string Token, string Email);