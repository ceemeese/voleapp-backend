using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        var hasher = new PasswordHasher<AuthUser>();
        var superAdminId = Guid.Parse("7c9e66ab-7839-47e2-9383-718693c04200");

        var superAdmin = new AuthUser
        {
            Id = superAdminId,
            UserName = "superadmin",
            NormalizedUserName = "SUPERADMIN",
            Email = "superadmin@voleapp.es",
            NormalizedEmail = "SUPERADMIN@VOLEAPP.ES",
            EmailConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        
        superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "SuperAdmin");
        
        builder.HasData(superAdmin);
    }
}