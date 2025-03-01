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
    public string UrlFormatString => "http://localhost:8456/videos/{0}";

    /// <inheritdoc />
    public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
    {
        return item is Movie;
    }
}