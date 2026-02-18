namespace Application.Abstractions.DTO;

public record LoginResponse(
    string Token,
    Guid RefreshToken
    );