using Blazored.LocalStorage;

namespace BlazorGameSpike.Client.State;

public class PersistedGameState(ILocalStorageService localStorage, ILogger<PersistedGameState> logger) : GameState, IDisposable
{
    private const string PersistenceKey = "GameStateSave";

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading game state from local storage");
        var loadedState = await localStorage.GetItemAsync<GameState>(PersistenceKey, cancellationToken);

        if (loadedState is not null)
        {
            Currency = loadedState.Currency;
            EnemiesDefeated = loadedState.EnemiesDefeated;
            EnemyHealth = loadedState.EnemyHealth;
            
            // Don't overwrite the upgrades, just update the levels
            foreach (var (upgrade, state) in loadedState.Upgrades)
            {
                Upgrades[upgrade].Level = state.Level;
            }
        }
        
        OnStateChange += HandleStateChange;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.SetItemAsync(PersistenceKey, this, cancellationToken);
        _lastSave = DateTimeOffset.Now;
    }

    private DateTimeOffset _lastSave = DateTimeOffset.Now;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private async void HandleStateChange()
    {
        await _saveLock.WaitAsync();
        
        try
        {
            if (DateTimeOffset.Now - _lastSave < TimeSpan.FromSeconds(2))
            {
                return;
            }
        
            logger.LogInformation("Game state changed, saving to local storage");
            await SaveAsync();
        }
        finally
        {
            _saveLock.Release();
        }
    }

    public void Dispose()
    {
        OnStateChange -= HandleStateChange;
    }
}