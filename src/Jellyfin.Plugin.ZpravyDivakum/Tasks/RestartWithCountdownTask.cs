using Jellyfin.Plugin.ZpravyDivakum.Services;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.ZpravyDivakum.Tasks;

/// <summary>
/// Scheduled task "Restart serveru s odpočtem" – lets Jellyfin's task scheduler restart the
/// server (e.g. every night) while warning everybody who is watching.
/// </summary>
public class RestartWithCountdownTask : IScheduledTask
{
    private readonly CountdownService _countdown;

    public RestartWithCountdownTask(CountdownService countdown)
    {
        _countdown = countdown;
    }

    public string Name => "Restart serveru s odpočtem";

    public string Key => "ZpravyDivakumRestart";

    public string Description => "Upozorní všechny diváky (3 min, 2 min, 1 min, 30 s, 20 s, 10 s) a restartuje Jellyfin. Když nikdo nic nepřehrává, restartuje hned.";

    public string Category => "Zprávy pro diváky";

    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        if (!_countdown.Start(null, true, null, "Naplánovaná úloha"))
        {
            return;
        }

        // Report progress until the restart (cancelling the task cancels the countdown).
        var total = Math.Max(1, _countdown.Status.SecondsLeft);
        try
        {
            while (_countdown.Status.Running)
            {
                progress.Report(100.0 * (total - _countdown.Status.SecondsLeft) / total);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            await _countdown.CancelAsync().ConfigureAwait(false);
        }
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers() => Array.Empty<TaskTriggerInfo>();
}
