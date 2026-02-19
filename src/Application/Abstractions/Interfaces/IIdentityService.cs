using Application.Abstractions.DTO;
using Domain.Roles;
using SharedKernel;

namespace Application.Abstractions.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string username, string email, string password);
    Task<Result<UserIdentity>> LoginAsync(string username, string password);
    Task<Result> UpdateUserProfileAsync(Guid userId, string oldUserName, string username, string email);
    Task<Result> UpdateUserPasswordAsync(Guid userId, string oldPassword, string newPassword);
    Task<Result> SetRoleAsync(IRole role, Guid userId);
    Task<Result<string?>> GetRolesAsync(Guid userId);
    Task<Result<bool>> IsInRoleAsync(Guid userId, string role);
    Task<Result> UpdateUserStatusAsync(Guid userId, bool isActive);
    Task<Result> SetRefreshTokenAsync(Guid userId, string refreshToken);
    Task<Result<UserIdentity>> ValidateRefreshToken(string refreshToken);
}