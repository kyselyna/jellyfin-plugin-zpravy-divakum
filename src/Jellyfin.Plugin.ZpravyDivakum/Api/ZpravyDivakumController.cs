using System.ComponentModel.DataAnnotations;
using Jellyfin.Plugin.ZpravyDivakum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.ZpravyDivakum.Api;

/// <summary>Message to send.</summary>
public sealed class SendRequest
{
    public string? Header { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    /// <summary>On-screen time in seconds (default from settings).</summary>
    public int? Seconds { get; set; }

    public Audience Audience { get; set; } = Audience.All;

    /// <summary>Session ids for <see cref="Audience.Selected"/>.</summary>
    public List<string>? SessionIds { get; set; }
}

/// <summary>Countdown parameters.</summary>
public sealed class CountdownRequest
{
    /// <summary>Total length in seconds (default = largest configured step).</summary>
    [Range(5, 86400)]
    public int? Seconds { get; set; }

    /// <summary>False = announce only, do not restart.</summary>
    public bool Restart { get; set; } = true;

    /// <summary>Custom text, {cas} is replaced with the remaining time.</summary>
    [MaxLength(2000)]
    public string? Text { get; set; }
}

/// <summary>Endpoints for the settings page (and for automations, e.g. Home Assistant).</summary>
[ApiController]
[Route("ZpravyDivakum")]
[Authorize(Policy = "RequiresElevation")]
public class ZpravyDivakumController : ControllerBase
{
    private readonly BroadcastService _broadcast;
    private readonly CountdownService _countdown;

    public ZpravyDivakumController(BroadcastService broadcast, CountdownService countdown)
    {
        _broadcast = broadcast;
        _countdown = countdown;
    }

    /// <summary>Connected clients.</summary>
    [HttpGet("Sessions")]
    public ActionResult<IReadOnlyList<ViewerSession>> Sessions() => Ok(_broadcast.Connected());

    /// <summary>Sends a message; returns how many clients got it.</summary>
    [HttpPost("Send")]
    public async Task<ActionResult<object>> Send([FromBody] SendRequest req, CancellationToken ct)
    {
        var cfg = Plugin.Instance!.Configuration;
        var sent = await _broadcast.SendAsync(
            string.IsNullOrWhiteSpace(req.Header) ? cfg.DefaultHeader : req.Header!,
            req.Text,
            req.Seconds ?? cfg.MessageSeconds,
            req.Audience,
            req.SessionIds,
            ct).ConfigureAwait(false);
        return Ok(new { sent });
    }

    [HttpGet("Countdown")]
    public ActionResult<CountdownStatus> Countdown() => Ok(_countdown.Status);

    /// <summary>Starts the restart countdown (409 when one is already running).</summary>
    [HttpPost("Countdown")]
    public ActionResult<CountdownStatus> StartCountdown([FromBody] CountdownRequest? req)
    {
        req ??= new CountdownRequest();
        var who = string.IsNullOrEmpty(User.Identity?.Name) ? "API" : User.Identity!.Name!;
        return _countdown.Start(req.Seconds, req.Restart, string.IsNullOrWhiteSpace(req.Text) ? null : req.Text, who)
            ? Ok(_countdown.Status)
            : Conflict(_countdown.Status);
    }

    [HttpDelete("Countdown")]
    public async Task<ActionResult<object>> CancelCountdown() =>
        Ok(new { cancelled = await _countdown.CancelAsync().ConfigureAwait(false) });
}
