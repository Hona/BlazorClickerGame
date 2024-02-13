using System.ComponentModel.Design;
using BlazorGameSpike.Client.Common;
using BlazorGameSpike.Client.State;

namespace BlazorGameSpike.Client.Scenes.Battles;

/// <summary>
/// Every 500ms impact the enemy with the current Passive Damage
/// </summary>
public class PassiveDamageWorker : GameStateComponent, IDisposable
{
    private Timer _timer;
    
    private const int BaseIntervalMs = 500;
    private bool _unlocked;
    private int _currentTimerPortalLevel;
    private TimeSpan GetInterval(int portalLevel)
    {
        var ms = BaseIntervalMs * Math.Pow(0.9, portalLevel);
        return TimeSpan.FromMilliseconds(Math.Max(ms, 1));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _currentTimerPortalLevel = 0;
        _unlocked = false;
        _timer = new Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
        GameState.OnStateChange += CheckDelayChange;
    }

    private void CheckDelayChange()
    {
        var portalLevel = GameState.Upgrades[Upgrade.Portal].Level;

        if (!_unlocked && GameState.Upgrades[Upgrade.Laser].Level > 0)
        {
            _unlocked = true;
            _timer.Change(GetInterval(portalLevel), GetInterval(portalLevel));
        }
        else
        {
            if (portalLevel != _currentTimerPortalLevel)
            {
                _currentTimerPortalLevel = portalLevel;
                _timer.Change(GetInterval(portalLevel), GetInterval(portalLevel));
            }
        }
    }

    private void OnTick(object? _)
    {
        GameState.PassiveDamageEnemy();
    }
    
    public new void Dispose()
    {
        _timer.Dispose();
        GameState.OnStateChange -= CheckDelayChange;
        
        base.Dispose();
    }
}