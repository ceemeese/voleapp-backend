namespace Domain.Common.ValueObjects;

public record PriceBreakdown(
      decimal BasePrice,
      decimal TotalPrice,
      decimal DiscountAmount,
      double AppliedDiscountPercent,
      string? DiscountReason
);