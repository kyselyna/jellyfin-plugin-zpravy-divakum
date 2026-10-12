using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.ZpravyDivakum.Configuration;

/// <summary>Plugin settings.</summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>Default header of ad-hoc messages.</summary>
    public string DefaultHeader { get; set; } = "Zpráva od správce";

    /// <summary>How long a message stays on screen (seconds).</summary>
    public int MessageSeconds { get; set; } = 10;

    /// <summary>Countdown steps in seconds before the restart, comma separated.</summary>
    public string CountdownSteps { get; set; } = "180,120,60,30,20,10";

    /// <summary>Header of countdown messages.</summary>
    public string CountdownHeader { get; set; } = "Restart serveru";

    /// <summary>Countdown text; {cas} = "3 minuty", "30 sekund"…</summary>
    public string CountdownText { get; set; } = "Server bude restartován za {cas}. Přehrávání se na chvíli přeruší.";

    /// <summary>Message sent when a countdown is cancelled.</summary>
    public string CancelText { get; set; } = "Restart serveru byl zrušen.";

    /// <summary>Restart right away (no countdown) when nobody is playing anything.</summary>
    public bool SkipCountdownWhenIdle { get; set; } = true;

    /// <summary>Sessions seen within this many seconds count as connected.</summary>
    public int ActiveWithinSeconds { get; set; } = 900;
}
