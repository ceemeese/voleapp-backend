using Domain.Club.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.DayOfWeek)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(s => s.OpeningTime)
            .HasColumnType("time")
            .IsRequired();
        
        builder.Property(s => s.ClosingTime)
            .HasColumnType("time")
            .IsRequired();
        
        builder.Property(s => s.IsClosed)
            .IsRequired();
    }
}