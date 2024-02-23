using System.Text.Json.Serialization;

namespace BlazorGameSpike.Client.State;

public record UpgradeState
{
    public static UpgradeState SeedNew(double baseCost, double growthFactor = 1.07) 
        => new()
        {
            BaseCost = baseCost,
            GrowthFactor = growthFactor,
            Level = 0
        };
    
    [JsonIgnore]
    public double CurrentCost => BaseCost * Math.Pow(GrowthFactor, Level);
    public int Level { get; set; }
    
    [JsonIgnore]
    public double BaseCost { get; init; }
    
    [JsonIgnore]
    public double GrowthFactor { get; init; }
};