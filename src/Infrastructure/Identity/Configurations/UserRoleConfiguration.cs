using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityUserRole<Guid>
            {
                UserId = Guid.Parse("7c9e66ab-7839-47e2-9383-718693c04200"),
                RoleId = Role.SuperAdmin.Id
            }
        );
    }
}