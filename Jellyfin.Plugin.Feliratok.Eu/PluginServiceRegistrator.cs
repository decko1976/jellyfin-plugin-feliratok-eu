using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Subtitles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Feliratok.Eu;

/// <summary>
/// Register subtitle provider.
/// </summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<ISubtitleProvider, FeliratokEuDownloader>();
        serviceCollection.AddSingleton<SubtitleFetcher>();
        serviceCollection.AddSingleton<SubtitleParser>();
    }
}
