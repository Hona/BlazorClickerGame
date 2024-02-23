using BlazorGameSpike.Client.State;
using Microsoft.AspNetCore.Components;

namespace BlazorGameSpike.Client.Common;

public abstract class GameStateComponent : ComponentBase, IDisposable
{
    [Inject] public PersistedGameState GameState { get; set; } = null!;

    protected override void OnInitialized()
    {
        GameState.OnStateChange += StateHasChanged;
    }

    public void Dispose()
    {
        GameState.OnStateChange -= StateHasChanged;
    }
}