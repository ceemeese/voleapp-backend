using Application.Abstractions.Errors;
using Application.Abstractions.Interfaces;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Identity;

internal sealed class IdentityService(
        RoleManager<IdentityRole> _roleManager,
        UserManager<AuthUser> _userManager,
        SignInManager<AuthUser> _signInManager
    ) : IIdentityService
{
    
    public async Task<Result<Guid>> CreateUserAsync(string username, string email, string password)
    {
        var user = new AuthUser(username)
        {
            Email = email
        };
        
        var result = await _userManager.CreateAsync(user, password);
        return result.ToApplicationResult(user.Id);
    }

    public async Task<Result<bool>> LoginAsync(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

        return result.Succeeded
            ? Result.Success(true)
            : Result.Failure<bool>(IdentityErrors.InvalidCredentials);
    }

    public async Task<Result> UpdateUserProfileAsync(Guid userId, string oldUserName, string username, string email)
    {
        if (oldUserName == "superadmin")
        {
            return Result.Failure(IdentityErrors.CannotUpdateSuperAdmin);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }
        user.UserName = username;
        user.Email = email;
        
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Result> UpdateUserPasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }
        
        if (user.UserName == "superadmin")
        {
            return Result.Failure(IdentityErrors.CannotUpdateSuperAdmin);
        }
        
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        return result.ToApplicationResult();
    }

    public async Task<Result<bool>> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure<bool>(IdentityErrors.NotFound(userId));
        }
        
        var result = await _userManager.IsInRoleAsync(user, role);
        return Result.Success(result);
    }

    /*public async Task SetRefreshTokenAsync(Guid userId, string refreshToken)
    {
        
    }*/
    
}