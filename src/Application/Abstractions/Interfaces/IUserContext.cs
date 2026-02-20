namespace Application.Abstractions.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    bool IsAdmin { get; }
    bool IsSuperAdmin { get; }
    bool IsAnyAdmin => IsAdmin || IsSuperAdmin;
}