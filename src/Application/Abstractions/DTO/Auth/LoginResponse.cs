namespace Application.Abstractions.DTO.Auth;

public sealed record LoginResponse(
    string Token,
    Guid RefreshToken
    );