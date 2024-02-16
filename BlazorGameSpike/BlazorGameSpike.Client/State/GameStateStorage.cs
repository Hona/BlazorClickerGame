namespace BlazorGameSpike.Client.State;

public class GameStateStorage
{
    public double Currency { get; set; }
    public int EnemiesDefeated { get; set; }
    private const int StartingEnemyHealth = 10;
    public int CurrentMaxEnemyHealth => (int)(StartingEnemyHealth * Math.Pow(1.01, EnemiesDefeated));
    public int EnemyHealth { get; set; } = StartingEnemyHealth;
    public Dictionary<Upgrade, UpgradeState> Upgrades { get; set; } =
        new ()
        {
            { Upgrade.Sword, UpgradeState.SeedNew(5) },
            { Upgrade.Laser, UpgradeState.SeedNew(15) },
            { Upgrade.Poison, UpgradeState.SeedNew(30, 1.11) },
            { Upgrade.Portal, UpgradeState.SeedNew(50, 1.12) },
        };
}