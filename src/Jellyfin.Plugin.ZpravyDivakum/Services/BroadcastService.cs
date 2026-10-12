using MediaBrowser.Controller;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Session;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.ZpravyDivakum.Services;

/// <summary>A connected client as shown on the settings page.</summary>
public sealed record ViewerSession(string Id, string UserName, string Client, string DeviceName, string? NowPlaying, DateTime LastActivity);

/// <summary>Who receives a message.</summary>
public enum Audience
{
    /// <summary>Every connected client.</summary>
    All,

    /// <summary>Only clients that are playing something.</summary>
    Watching,

    /// <summary>Explicitly listed session ids.</summary>
    Selected,
}

/// <summary>Sends on-screen messages to Jellyfin clients.</summary>
public sealed class BroadcastService
{
    private readonly ISessionManager _sessions;
    private readonly ILogger<BroadcastService> _logger;

    public BroadcastService(ISessionManager sessions, ILogger<BroadcastService> logger)
    {
        _sessions = sessions;
        _logger = logger;
    }

    private static Configuration.PluginConfiguration Config => Plugin.Instance?.Configuration ?? new Configuration.PluginConfiguration();

    /// <summary>Connected user sessions (no API keys, no server-internal sessions).</summary>
    public IReadOnlyList<ViewerSession> Connected() =>
        Candidates().Select(s => new ViewerSession(s.Id, s.UserName ?? "?", s.Client ?? "?", s.DeviceName ?? "?", s.NowPlayingItem?.Name is { } n ? Title(s) : null, s.LastActivityDate))
            .OrderByDescending(s => s.NowPlaying is not null)
            .ThenBy(s => s.UserName)
            .ToList();

    public bool AnyonePlaying() => Candidates().Any(s => s.NowPlayingItem is not null);

    /// <returns>Number of clients the message was delivered to.</returns>
    public async Task<int> SendAsync(string header, string text, int seconds, Audience audience, IReadOnlyCollection<string>? sessionIds, CancellationToken ct)
    {
        var targets = Candidates().Where(s => audience switch
        {
            Audience.Watching => s.NowPlayingItem is not null,
            Audience.Selected => sessionIds?.Contains(s.Id) == true,
            _ => true,
        }).ToList();

        var command = new MessageCommand { Header = header, Text = text, TimeoutMs = Math.Max(1, seconds) * 1000L };
        var sent = 0;
        foreach (var s in targets)
        {
            try
            {
                await _sessions.SendMessageCommand(null!, s.Id, command, ct).ConfigureAwait(false);
                sent++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogDebug(ex, "Message to {Client} ({User}) failed", s.Client, s.UserName);
            }
        }

        _logger.LogInformation("Message \"{Header}\" sent to {Sent}/{Total} clients", header, sent, targets.Count);
        return sent;
    }

    private IEnumerable<SessionInfo> Candidates()
    {
        var since = DateTime.UtcNow.AddSeconds(-Math.Max(60, Config.ActiveWithinSeconds));
        return _sessions.Sessions.Where(s => s.UserId != Guid.Empty && s.LastActivityDate >= since);
    }

    private static string Title(SessionInfo s)
    {
        var i = s.NowPlayingItem;
        return string.IsNullOrEmpty(i.SeriesName) ? i.Name : $"{i.SeriesName} – {i.Name}";
    }
}
