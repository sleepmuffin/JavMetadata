using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.JavMetadata.Providers.MetadataServe;

public class MetadataServeExternalId : IExternalId
{
    /// <inheritdoc />
    public string ProviderName => "MetadataServe";

    /// <inheritdoc />
    public string Key => "MetadataServe";

    /// <inheritdoc />
    public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
    {
        return item is Movie;
    }
}

public class MetadataServeExternalUrlProvider : IExternalUrlProvider
{
    /// <inheritdoc />
    public string Name => "MetadataServe";

    /// <inheritdoc />
    public IEnumerable<string> GetExternalUrls(BaseItem item)
    {
        if (item is Movie
            && item.TryGetProviderId("MetadataServe", out var id)
            && !string.IsNullOrEmpty(id))
        {
            yield return $"http://192.168.0.100:8456/videos/{id}";
        }
    }
}