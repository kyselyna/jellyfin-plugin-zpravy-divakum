using MediaBrowser.Controller;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.ZpravyDivakum.Services;

/// <summary>State of the running countdown.</summary>
public sealed record CountdownStatus(bool Running, DateTime? RestartAt, int SecondsLeft, bool Restart, string? StartedBy);

/// <summary>
/// Countdown before a server restart: announces the remaining time at the configured steps
/// (default 3 min, 2 min, 1 min, 30 s, 20 s, 10 s) and then restarts Jellyfin.
/// </summary>
public sealed class CountdownService : IDisposable
{
    private readonly BroadcastService _broadcast;
    private readonly ISystemManager _system;
    private readonly ILogger<CountdownService> _logger;
    private readonly object _lock = new();
    private CancellationTokenSource? _cts;
    private DateTime? _restartAt;
    private bool _restart;
    private string? _startedBy;

    public CountdownService(BroadcastService broadcast, ISystemManager system, ILogger<CountdownService> logger)
    {
        _broadcast = broadcast;
        _system = system;
        _logger = logger;
    }

    private static Configuration.PluginConfiguration Config => Plugin.Instance?.Configuration ?? new Configuration.PluginConfiguration();

    public CountdownStatus Status
    {
        get
        {
            lock (_lock)
            {
                var left = _restartAt is { } at ? Math.Max(0, (int)Math.Ceiling((at - DateTime.UtcNow).TotalSeconds)) : 0;
                return new CountdownStatus(_cts is not null, _restartAt, left, _restart, _startedBy);
            }
        }
    }

    /// <summary>Starts the countdown.</summary>
    /// <param name="totalSeconds">Length; null = largest configured step.</param>
    /// <param name="restart">False = only announce (e.g. a test), do not restart.</param>
    /// <param name="text">Custom text with {cas}; null = configured text.</param>
    /// <returns>False when a countdown is already running.</returns>
    public bool Start(int? totalSeconds, bool restart, string? text, string startedBy)
    {
        var steps = Steps();
        var total = totalSeconds ?? (steps.Count > 0 ? steps[0] : 180);
        if (Config.SkipCountdownWhenIdle && restart && !_broadcast.AnyonePlaying())
        {
            total = Math.Min(total, 5);
        }

        lock (_lock)
        {
            if (_cts is not null)
            {
                return false;
            }

            _cts = new CancellationTokenSource();
            _restartAt = DateTime.UtcNow.AddSeconds(total);
            _restart = restart;
            _startedBy = startedBy;
        }

        var token = _cts.Token;
        _ = Task.Run(() => RunAsync(total, steps.Where(s => s <= total).ToList(), restart, text ?? Config.CountdownText, token), CancellationToken.None);
        _logger.LogInformation("Restart countdown {Seconds} s started by {User} (restart: {Restart})", total, startedBy, restart);
        return true;
    }

    /// <returns>False when nothing was running.</returns>
    public async Task<bool> CancelAsync()
    {
        CancellationTokenSource? cts;
        lock (_lock)
        {
            cts = _cts;
            _cts = null;
            _restartAt = null;
        }

        if (cts is null)
        {
            return false;
        }

        await cts.CancelAsync().ConfigureAwait(false);
        cts.Dispose();
        await _broadcast.SendAsync(Config.CountdownHeader, Config.CancelText, Config.MessageSeconds, Audience.All, null, CancellationToken.None).ConfigureAwait(false);
        _logger.LogInformation("Restart countdown cancelled");
        return true;
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    /// <summary>Configured steps, largest first.</summary>
    public static List<int> Steps() =>
        Config.CountdownSteps.Split(',', ';', ' ')
            .Select(s => int.TryParse(s.Trim(), out var v) ? v : 0)
            .Where(v => v > 0)
            .Distinct()
            .OrderByDescending(v => v)
            .ToList();

    private async Task RunAsync(int total, List<int> steps, bool restart, string text, CancellationToken ct)
    {
        try
        {
            var end = DateTime.UtcNow.AddSeconds(total);
            if (!steps.Contains(total) && total > 5)
            {
                steps.Insert(0, total);
            }

            for (var i = 0; i < steps.Count; i++)
            {
                var wait = end.AddSeconds(-steps[i]) - DateTime.UtcNow;
                if (wait > TimeSpan.Zero)
                {
                    await Task.Delay(wait, ct).ConfigureAwait(false);
                }

                // keep each message on screen until the next one (max 15 s)
                var next = i + 1 < steps.Count ? steps[i] - steps[i + 1] : steps[i];
                var show = Math.Clamp(next, 3, 15);
                await _broadcast.SendAsync(Config.CountdownHeader, text.Replace("{cas}", CzechTime.Format(steps[i]), StringComparison.Ordinal), show, Audience.All, null, ct).ConfigureAwait(false);
            }

            var rest = end - DateTime.UtcNow;
            if (rest > TimeSpan.Zero)
            {
                await Task.Delay(rest, ct).ConfigureAwait(false);
            }

            if (restart)
            {
                await _broadcast.SendAsync(Config.CountdownHeader, "Restartuji server…", 5, Audience.All, null, ct).ConfigureAwait(false);
                await Task.Delay(1500, ct).ConfigureAwait(false);
                _logger.LogWarning("Restarting Jellyfin after countdown");
                _system.Restart();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Restart countdown failed");
        }
        finally
        {
            lock (_lock)
            {
                if (_cts?.Token == ct)
                {
                    _cts.Dispose();
                    _cts = null;
                    _restartAt = null;
                }
            }
        }
    }
}
