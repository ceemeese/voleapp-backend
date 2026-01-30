using Domain.Club;
using Domain.Club.Entities;
using Domain.Reservation;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(r => r.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(r => r.Date);
        
        builder.HasIndex(r => new {r.CourtId, r.Date});
        
        builder.Property(r => r.StartTime)
            .HasColumnType("time")
            .IsRequired();
        
        builder.Property(r => r.EndTime)
            .HasColumnType("time")
            .IsRequired();
        
        builder.Property(r => r.TotalPrice)
            .HasPrecision(10,2)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(300);
        
        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(r => r.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Club>()
            .WithMany()
            .HasForeignKey(r => r.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Court>()
            .WithMany()
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}