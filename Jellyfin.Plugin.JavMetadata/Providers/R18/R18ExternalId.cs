using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.JavMetadata.Providers.R18;

public class R18ExternalId : IExternalId
{
    /// <inheritdoc />
    public string ProviderName => "R18";

    /// <inheritdoc />
    public string Key => "R18";

    /// <inheritdoc />
    public string UrlFormatString => "https://r18.dev/videos/vod/movies/detail/-/id={0}/";

    /// <inheritdoc />
    public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
    {
        return item is Movie;
    }
}