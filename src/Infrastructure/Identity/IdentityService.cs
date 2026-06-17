using Application.Abstractions.DTO.Auth;
using Application.Abstractions.Errors;
using Application.Abstractions.Interfaces;
using Domain.Roles;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Identity;

internal sealed class IdentityService(
        RoleManager<IdentityRole<Guid>> _roleManager,
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
        if (!result.Succeeded)
        {
            return result.ToApplicationResult(user.Id);
        }
        
        var roleResult = await _userManager.AddToRoleAsync(user, Role.User.Name);
        
        return roleResult.ToApplicationResult(user.Id);
    }

    
    public async Task<Result<UserIdentity>> LoginAsync(string loginInput, string password)
    {
        var user = await _userManager.FindByEmailAsync(loginInput)
            ?? await _userManager.FindByNameAsync(loginInput);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Result.Failure<UserIdentity>(IdentityErrors.InvalidCredentials);
        }

        if (!user.EmailConfirmed)
        {
            return Result.Failure<UserIdentity>(IdentityErrors.EmailNotConfirmed);
        }
        
        var resultRoles = await _userManager.GetRolesAsync(user);
        var role = resultRoles.FirstOrDefault() ?? Role.User.Name;
        
        return Result.Success(new UserIdentity(user.Id, user.Email!, role));
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
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
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
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
    }


    public async Task<Result> SetRoleAsync(IRole role, Guid userId)
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
        
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
        }
        
        var result = await _userManager.AddToRoleAsync(user, role.Name);
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
    }

    public async Task<Result<string?>> GetRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure<string?>(IdentityErrors.NotFound(userId));
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        return Result.Success(role);
    }

    public async Task<Result<bool>> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure<bool>(IdentityErrors.NotFound(userId));
        }
        
        var isInRole = await _userManager.IsInRoleAsync(user, role);
        return Result.Success(isInRole);
    }

    public async Task<Result> UpdateUserStatusAsync(Guid userId, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }

        IdentityResult result;
        
        if (!isActive)
        {
            //bloqueo
            await _userManager.SetLockoutEnabledAsync(user, true);
            result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
        else
        {
            //desbloqueo
            result = await _userManager.SetLockoutEndDateAsync(user, null);
        }
        
        return result.ToApplicationResult(IdentityErrors.UpdateFailed); 
    }

    public async Task<Result> SetRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
    }

    public async Task<Result<UserIdentity>> ValidateRefreshToken(string refreshToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
        {
            return Result.Failure<UserIdentity>(IdentityErrors.InvalidToken);
        }

        if (user.RefreshTokenExpires < DateTime.UtcNow)
        {
            return Result.Failure<UserIdentity>(IdentityErrors.SessionExpired);
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? Role.User.Name;

        return Result.Success(new UserIdentity(user.Id, user.Email!, role!));
    }

    public async Task<ForgotPasswordIdentity?> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return new ForgotPasswordIdentity(token, user.Email!);
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure(IdentityErrors.NotFoundByEmail);
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
    }
    
    public async Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }
        
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        
        return result.ToApplicationResult(IdentityErrors.UpdateFailed);
    }

    public async Task<EmailConfirmationTokenIdentity?> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return new EmailConfirmationTokenIdentity(user.Email!, token);
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure(IdentityErrors.NotFoundByEmail);
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        
        return result.ToApplicationResult(IdentityErrors.ConfirmEmailFailed);
    }
   
    public async Task<EmailConfirmationTokenIdentity?> ResendConfirmationEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var resultConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (resultConfirmed)
        {
            return null;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return new EmailConfirmationTokenIdentity(user.Email!, token);
    }
}