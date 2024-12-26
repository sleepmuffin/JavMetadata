using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.JavMetadata.Providers.R18Dev;

public class R18DevExternalId : IExternalId
{
    /// <inheritdoc />
    public string ProviderName => "R18Dev";

    /// <inheritdoc />
    public string Key => "R18Dev";

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