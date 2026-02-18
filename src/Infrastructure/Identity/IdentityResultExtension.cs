using Application.Abstractions.Errors;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Identity;

internal static class IdentityResultExtension
{
    public static Result<Guid> ToApplicationResult(this IdentityResult result, Guid userId)
    {
        if (result.Succeeded)
        {
            return Result.Success(userId);
        }
        
        var error = result.Errors.FirstOrDefault();

        return error!.Code switch
        {
            "DuplicateUserName" =>
                Result.Failure<Guid>(IdentityErrors.UserNameNotUnique),
            "DuplicateEmail" =>
                Result.Failure<Guid>(IdentityErrors.EmailNotUnique),
            _ => Result.Failure<Guid>(IdentityErrors.RegistrationFailed)
        };
    }
    
    public static Result ToApplicationResult(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return Result.Success();
        }
        
        var error = result.Errors.FirstOrDefault();

        return error!.Code switch
        {
            "DuplicateUserName" =>
                Result.Failure<Guid>(IdentityErrors.UserNameNotUnique),
            "DuplicateEmail" =>
                Result.Failure<Guid>(IdentityErrors.EmailNotUnique),
            _ => Result.Failure<Guid>(IdentityErrors.RegistrationFailed)
        };
    }
}