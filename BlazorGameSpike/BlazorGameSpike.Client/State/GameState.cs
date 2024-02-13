namespace BlazorGameSpike.Client.State;

public class GameState
{
    public event Action? OnStateChange;

    public double Currency { get; private set; }
    public int EnemiesDefeated { get; private set; }

    public IReadOnlyDictionary<Upgrade, UpgradeState> Upgrades { get; private set; } =
        new Dictionary<Upgrade, UpgradeState>
        {
            { Upgrade.Sword, UpgradeState.SeedNew(5) },
            { Upgrade.Laser, UpgradeState.SeedNew(15) },
            { Upgrade.Poison, UpgradeState.SeedNew(30, 1.11) },
            { Upgrade.Portal, UpgradeState.SeedNew(50, 1.12) },
        };
    
    public void AddCurrency(double amount)
    {
        Currency += amount;
        InvokeStateChange();
    }
    
    private const int StartingEnemyHealth = 10;
    public int CurrentMaxEnemyHealth => (int)(StartingEnemyHealth * Math.Pow(1.01, EnemiesDefeated));
    public int EnemyHealth { get; private set; } = StartingEnemyHealth;

    public void ClickDamageEnemy()
    {
        const int BaseDamage = 1;

        var actualDamage = BaseDamage 
                           + Upgrades[Upgrade.Sword].Level
                           + (Upgrades[Upgrade.Poison].Level * 0.1 * PassiveDamage);
        
        DamageEnemy((int)actualDamage);
    }

    private int PassiveDamage => Upgrades[Upgrade.Laser].Level;
    public void PassiveDamageEnemy()
    {
        DamageEnemy(PassiveDamage);
    }
    
    private void DamageEnemy(int amount)
    {
        EnemyHealth -= amount;
        InvokeStateChange();

        if (EnemyHealth < 0)
        {
            DefeatEnemy();
            AddCurrency(1);
        }
    }
    
    private void DefeatEnemy()
    {
        EnemiesDefeated++;
        
        EnemyHealth = CurrentMaxEnemyHealth;
    }
    
    public void PurchaseUpgrade(Upgrade upgrade)
    {
        var upgradeState = Upgrades[upgrade];
        
        if (Currency >= upgradeState.CurrentCost)
        {
            Currency -= upgradeState.CurrentCost;
            upgradeState.Level++;
            InvokeStateChange();
        }
    }
    
    // TODO: This should be debounced in a real app
    private void InvokeStateChange() => OnStateChange?.Invoke();
}