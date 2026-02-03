using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models;

public sealed class AuthUser : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }

    public AuthUser(string username) : base(username)
    {
    }

    public AuthUser() : base()
    {
    }
}