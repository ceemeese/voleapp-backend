using Domain.Club;
using Domain.Club.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.HasKey(club => club.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasIndex(c => c.Name)
            .IsUnique();
        
        builder.Property(c => c.Cif)
            .HasMaxLength(15)
            .IsRequired();
        
        builder.HasIndex(c => c.Cif)
            .IsUnique();
        
        //RECUERDA! value object
        builder.OwnsOne(c => c.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasMaxLength(300)
                .IsRequired();
            addressBuilder.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();
            addressBuilder.Property(a => a.ZipCode)
                .HasMaxLength(10)
                .IsRequired();
            addressBuilder.Property(a => a.Country)
                .HasMaxLength(100)
                .IsRequired();
        });
        
        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(c => c.Email)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.IsActive)
            .IsRequired();
        
        builder.HasIndex(c => c.IsActive);
        
        builder.Property(c => c.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.HasMany(c => c.Schedules)
            .WithOne()
            .HasForeignKey(schedule => schedule.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(c => c.Schedules)
            .HasField("_schedules")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(c => c.Members)
            .WithOne()
            .HasForeignKey(member => member.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(c => c.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasOne(c => c.PricingConfig)
            .WithOne()
            .HasForeignKey<PricingConfig>(p => p.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}