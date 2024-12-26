using System.Globalization;
using System.Net.Http.Headers;
using Jellyfin.Plugin.JavMetadata.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.JavMetadata;

// REGEX
// ([A-Z-]+[\d]+)
// HODV-21619.mp4
// [SAME-150]
// hhd800.com@IPX-916.mp4
// hhd800.com@FC2-PPV-4593593.mp4
// hhd800.com@SDMU-963_Uncensored_Leaked.mp4
// hhd800.com@SONE-499.mp4
// hhd800.com@FSDSS-946.mp4
// hhd800.com@HEYZO-3481.mp4

/// <summary>
///     The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly IHttpClientFactory _httpClientFactory;

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer,
        IHttpClientFactory httpClientFactory) : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _httpClientFactory = httpClientFactory;
    }

    public override string Name => Constants.PluginName;
    public override Guid Id => Guid.NewGuid();

    /// <summary>
    ///     Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        ];
    }

    public HttpClient GetHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);
        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("JAVMetadata", Version.ToString()));

        return httpClient;
    }

    /// <summary>
    ///     Register webhook services.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
        }
    }
}