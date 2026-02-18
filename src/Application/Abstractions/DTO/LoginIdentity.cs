namespace Application.Abstractions.DTO;

public sealed record LoginIdentity(Guid UserId, string Email, string Role);