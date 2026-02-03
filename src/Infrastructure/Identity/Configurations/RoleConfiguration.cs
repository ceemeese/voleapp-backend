using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Role.SuperAdmin.Id,
                Name = Role.SuperAdmin.Name,
                NormalizedName = Role.SuperAdmin.Name.ToUpper()
            },
            new IdentityRole<Guid>
            {
                Id = Role.Admin.Id,
                Name = Role.Admin.Name,
                NormalizedName = Role.Admin.Name.ToUpper()
            },
            new IdentityRole<Guid>
            {
                Id = Role.User.Id,
                Name = Role.User.Name,
                NormalizedName = Role.User.Name.ToUpper()
            }
        );
    }
}