using Domain.Common;
using SharedKernel;

namespace Domain.Club.Entities;

public sealed class PricingConfig : Entity<int>
{
    public Guid ClubId { get; private set; }
    public decimal RainDiscountPercent { get; private set; }
    public double WindThreshold { get; private set; }
    public decimal WindDiscountPercent { get; private set; }
    public double HeatThreshold { get; private set; }
    public decimal HeatDiscountPercent { get; private set; }
    public double ColdThreshold { get; private set; }
    public decimal ColdDiscountPercent { get; private set; }

    internal PricingConfig(Guid clubId)
    {
        ClubId = clubId;
        RainDiscountPercent = 0;
        WindThreshold = 30;
        WindDiscountPercent = 0;
        HeatThreshold = 30;
        HeatDiscountPercent = 0;
        ColdThreshold = 5;
        ColdDiscountPercent = 0;
    }
    
    private PricingConfig() { }
    
    public Result Update(
        decimal rainD, 
        double windT, decimal windD, 
        double heatT, decimal heatD, 
        double coldT, decimal coldD)
    {

        var validationResult = ValidatePercentages(rainD, windD, heatD, coldD);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        RainDiscountPercent = rainD;
        WindThreshold = windT;
        WindDiscountPercent = windD;
        HeatThreshold = heatT;
        HeatDiscountPercent = heatD;
        ColdThreshold = coldT;
        ColdDiscountPercent = coldD;

        return Result.Success();
    }
    
    private Result ValidatePercentages(params decimal[] percentages)
    {
        foreach (var percent in percentages)
        {
            if (percent < 0 || percent > 100)
            {
                return Result.Failure(PricingConfigErrors.InvalidPercentage);
            }
        }
        return Result.Success();
    }
    
}