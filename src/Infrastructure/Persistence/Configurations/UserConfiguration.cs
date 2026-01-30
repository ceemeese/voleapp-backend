using Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

// EF puede leer igualmente la clase porque utiliza reflexion 
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    //método público porque implementa interpazpública IEntityType, se debe dar permiso a EF para llamarlo
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Dni)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.HasIndex(u => u.Dni)
            .IsUnique();
        
        builder.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.Name)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();
        
        builder.Property(u => u.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        //filtro de consulta global, siempre uso activos
        builder.HasQueryFilter(u => u.IsActive);
    }
}