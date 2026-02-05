using SharedKernel;

namespace Application.Abstractions.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string username, string email, string password);
    Task<Result<bool>> LoginAsync(string username, string password);
    Task<Result> UpdateUserProfileAsync(Guid userId, string oldUserName, string username, string email);
    Task<Result> UpdateUserPasswordAsync(Guid userId, string oldPassword, string newPassword);
    Task<Result<bool>> IsInRoleAsync(Guid userId, string role);
    //Task SetRefreshTokenAsync(Guid userId, string refreshToken);
}