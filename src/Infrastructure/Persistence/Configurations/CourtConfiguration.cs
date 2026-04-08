using Domain.Club;
using Domain.Court;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.HasOne<Club>()
            .WithMany()
            .HasForeignKey(c => c.ClubId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(c => c.BasePrice)
            .HasPrecision(10,2)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.HasMany(c => c.CourtEvents)
            .WithOne()
            .HasForeignKey(ce => ce.CourtId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(c => c.CourtEvents)
            .HasField("_courtEvents")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        //filtro global
        //builder.HasQueryFilter(c => c.IsActive);
    }
}