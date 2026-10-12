using Jellyfin.Plugin.ZpravyDivakum.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.ZpravyDivakum;

/// <summary>Registers the plugin's services.</summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<BroadcastService>();
        serviceCollection.AddSingleton<CountdownService>();
    }
}
