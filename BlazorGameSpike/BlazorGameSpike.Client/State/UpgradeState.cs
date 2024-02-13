namespace BlazorGameSpike.Client.State;

public record UpgradeState(
    int Level,
    double BaseCost,
    double GrowthFactor)
{
    public static UpgradeState SeedNew(double baseCost, double growthFactor = 1.07) 
        => new(0, baseCost, growthFactor);
    
    public double CurrentCost => BaseCost * Math.Pow(GrowthFactor, Level);
    public int Level { get; set; } = Level;
};