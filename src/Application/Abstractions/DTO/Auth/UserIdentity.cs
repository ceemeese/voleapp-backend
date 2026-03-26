namespace Application.Abstractions.DTO.Auth;

public sealed record UserIdentity(Guid UserId, string Email, string Role);