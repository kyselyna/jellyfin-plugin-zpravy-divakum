using Jellyfin.Plugin.ZpravyDivakum.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.ZpravyDivakum;

/// <summary>
/// Zprávy pro diváky – messages to everyone watching and a server restart with a countdown.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public static Plugin? Instance { get; private set; }

    public override string Name => "Zprávy pro diváky";

    public override Guid Id => Guid.Parse("0f48fa7b-83f3-41ad-8114-70864ed7ae01");

    public override string Description =>
        "Pošle zprávu všem, kdo se právě dívají, a restartuje server s odpočtem (3 min, 2 min, 1 min, 30 s, 20 s, 10 s).";

    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = "ZpravyDivakum",
            DisplayName = "Zprávy pro diváky",
            EnableInMainMenu = true,
            EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html",
        };
    }
}
