namespace Application.Abstractions.DTO.Auth;

public sealed record EmailConfirmationTokenIdentity(string Email, string Token);