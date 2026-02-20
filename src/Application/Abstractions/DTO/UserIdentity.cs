namespace Application.Abstractions.DTO;

public sealed record UserIdentity(Guid UserId, string Email, string Role);