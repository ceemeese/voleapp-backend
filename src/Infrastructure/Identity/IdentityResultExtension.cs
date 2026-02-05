using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Identity;

internal static class IdentityResultExtension
{
    public static Result<Guid> ToApplicationResult(this IdentityResult result, Guid userId)
    {
        return result.Succeeded
            ? Result.Success(userId)
            : Result.Failure<Guid>(new Error(result.Errors.First().Code,result.Errors.First().Description, ErrorType.Failure));
    }
    
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(new Error(result.Errors.First().Code,result.Errors.First().Description, ErrorType.Failure));
    }
}