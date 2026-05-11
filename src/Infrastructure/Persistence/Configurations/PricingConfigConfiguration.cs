using Domain.Club.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PricingConfigConfiguration : IEntityTypeConfiguration<PricingConfig>
{
    public void Configure(EntityTypeBuilder<PricingConfig> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        
        builder.Property(p => p.RainDiscountPercent).HasPrecision(5, 2);
        builder.Property(p => p.ColdDiscountPercent).HasPrecision(5, 2);
        builder.Property(p => p.HeatDiscountPercent).HasPrecision(5, 2);
        builder.Property(p => p.WindDiscountPercent).HasPrecision(5, 2);
    }
}