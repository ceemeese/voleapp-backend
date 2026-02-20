namespace Application.Abstractions.DTO;

public sealed record LoginResponse(
    string Token,
    Guid RefreshToken
    );