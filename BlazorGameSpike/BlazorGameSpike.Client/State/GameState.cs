using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace BlazorGameSpike.Client.State;


public class GameState(ILocalStorageService localStorageService)
{
    public event Action? OnStateChange;

    public GameStateStorage GameStateStorage { get; } = new();
    
    public async Task Load()
    {
        var status = await localStorageService.GetItemAsync<string>("Status");
        if (string.IsNullOrEmpty(status))
            return;
        
        var state = JsonSerializer.Deserialize<GameStateStorage>(status);
        if (state == null)
            return;
        
        GameStateStorage.Currency = state.Currency;
        GameStateStorage.Upgrades = state.Upgrades;
        GameStateStorage.EnemyHealth = state.EnemyHealth;
        GameStateStorage.EnemiesDefeated = state.EnemiesDefeated;
        
        InvokeStateChange();
    }
    

    private void AddCurrency(double amount)
    {
        GameStateStorage.Currency += amount;
        InvokeStateChange();
    }

    public void ClickDamageEnemy()
    {
        const int baseDamage = 1;

        var actualDamage = baseDamage 
                           + GameStateStorage.Upgrades[Upgrade.Sword].Level
                           + (GameStateStorage.Upgrades[Upgrade.Poison].Level * 0.1 * PassiveDamage);
        
        DamageEnemy((int)actualDamage);
    }

    private async Task Save()
    {
        var status = JsonSerializer.Serialize(GameStateStorage);
        await localStorageService.SetItemAsync("Status", status);
    }

    private int PassiveDamage => GameStateStorage.Upgrades[Upgrade.Laser].Level;
    public void PassiveDamageEnemy()
    {
        DamageEnemy(PassiveDamage);
    }
    
    private void DamageEnemy(int amount)
    {
        GameStateStorage.EnemyHealth -= amount;
        InvokeStateChange();

        if (GameStateStorage.EnemyHealth < 0)
        {
            DefeatEnemy();
            AddCurrency(1);
        }
    }
    
    private void DefeatEnemy()
    {
        GameStateStorage.EnemiesDefeated++;
        
        GameStateStorage.EnemyHealth = GameStateStorage.CurrentMaxEnemyHealth;
    }
    
    public void PurchaseUpgrade(Upgrade upgrade)
    {
        var upgradeState = GameStateStorage.Upgrades[upgrade];
        
        if (GameStateStorage.Currency >= upgradeState.CurrentCost)
        {
            GameStateStorage.Currency -= upgradeState.CurrentCost;
            upgradeState.Level++;
            InvokeStateChange();
        }
    }
    
    // TODO: This should be debounced in a real app
    private async void InvokeStateChange()
    {
        await Save();
        OnStateChange?.Invoke();
    }
}