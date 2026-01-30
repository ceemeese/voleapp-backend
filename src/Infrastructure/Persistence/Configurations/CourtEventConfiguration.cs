using Domain.Club.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class CourtEventConfiguration : IEntityTypeConfiguration<CourtEvent>
{
    public void Configure(EntityTypeBuilder<CourtEvent> builder)
    {
        builder.HasKey(ce => ce.Id);
        
        builder.Property(ce => ce.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ce => ce.StartTime)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(ce => ce.EndTime)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(ce => ce.EventName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ce => ce.Description)
            .HasMaxLength(500);
        
        builder.Property(ce => ce.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired()
            .ValueGeneratedOnAdd();
    }
}