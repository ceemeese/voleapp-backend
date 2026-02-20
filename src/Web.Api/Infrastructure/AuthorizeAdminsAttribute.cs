using Domain.Roles;
using Microsoft.AspNetCore.Authorization;

namespace Web.Api.Infrastructure;

internal sealed class AuthorizeAdminsAttribute : AuthorizeAttribute
{
    public AuthorizeAdminsAttribute()
    {
        Roles = $"{Role.SuperAdmin.Name},{Role.Admin.Name}";
    }
}

internal sealed class AuthorizeSuperAdminAttribute : AuthorizeAttribute
{
    public AuthorizeSuperAdminAttribute()
    {
        Roles = $"{Role.SuperAdmin.Name}";
    }
}