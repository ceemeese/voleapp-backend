using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ClubMemberConfiguration : IEntityTypeConfiguration<ClubMember>
{
    public void Configure(EntityTypeBuilder<ClubMember> builder)
    {
        builder.HasKey(cm => cm.Id);
        builder.Property(cm => cm.Id)
            .ValueGeneratedOnAdd();
        
        //un usuario no puede estar duplicado en un club
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Club>()
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(cm => new {cm.ClubId, cm.UserId})
            .IsUnique();
        
        builder.Property(cm => cm.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(cm => cm.MembershipNumber)
            .HasMaxLength(100);
        
        builder.HasIndex(cm => cm.MembershipNumber)
            .IsUnique();

        builder.Property(cm => cm.RegisteredOn)
            .HasColumnType("date")
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.Property(cm => cm.IsFavourite)
            .IsRequired();
        
        builder.Property(cm => cm.IsMember)
            .IsRequired();

        builder.Property(cm => cm.IsActive)
            .IsRequired();
        
        //ver miembros activos
        //builder.HasQueryFilter(cm => cm.IsActive);
    }
}