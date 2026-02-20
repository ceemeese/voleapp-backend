using Application.Abstractions.Interfaces;

namespace Application.Abstractions.Extensions;

internal static class UserContextExtension
{
    public static bool IsOwnerOrSuperadmin(this IUserContext userContext, Guid userId)
    {
        return userContext.UserId == userId ||  userContext.IsSuperAdmin;
    }

    public static bool IsAnyAdmin(this IUserContext userContext)
    {
        return userContext.IsAdmin || userContext.IsSuperAdmin;
    }

    public static bool IsOnlySuperadmin(this IUserContext userContext)
    {
        return userContext.IsSuperAdmin;
    }
}