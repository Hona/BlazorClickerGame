using BlazorGameSpike.Client.State;
using Microsoft.AspNetCore.Components;

namespace BlazorGameSpike.Client.Common;

public abstract class GameStateComponent : ComponentBase, IDisposable
{
    [Inject] public GameState GameState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        GameState.OnStateChange += StateHasChanged;
        await GameState.Load();
    }

    public void Dispose()
    {
        GameState.OnStateChange -= StateHasChanged;
    }
}