using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace BlazorGameSpike.Client.State;


public class GameState
{
    public event Action? OnStateChange;
    public double Currency { get; set; }
    public int EnemiesDefeated { get; set; }
    private const int StartingEnemyHealth = 10;
    [JsonIgnore]
    public int CurrentMaxEnemyHealth => (int)(StartingEnemyHealth + EnemiesDefeated * 0.1);
    public int EnemyHealth { get; set; } = StartingEnemyHealth;
    public Dictionary<Upgrade, UpgradeState> Upgrades { get; set; } =
        new ()
        {
            { Upgrade.Sword, UpgradeState.SeedNew(5) },
            { Upgrade.Laser, UpgradeState.SeedNew(15) },
            { Upgrade.Poison, UpgradeState.SeedNew(30, 1.08) },
            { Upgrade.Portal, UpgradeState.SeedNew(50, 1.09) },
        };

    private void AddCurrency(double amount)
    {
        Currency += amount;
        InvokeStateChange();
    }

    public void ClickDamageEnemy()
    {
        const int baseDamage = 1;

        var actualDamage = baseDamage 
                           + Upgrades[Upgrade.Sword].Level
                           + Upgrades[Upgrade.Poison].Level * 0.1 * PassiveDamage;
        
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
        
        InvokeStateChange();
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