using System.Text.Json;
using System.Text.RegularExpressions;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JavMetadata.Providers.R18Dev;

public class R18DevRemoteProvider<B, T, E> : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    where T : BaseItem, IHasLookupInfo<E>
    where E : ItemLookupInfo, new()
{
    protected readonly IServerConfigurationManager _config;
    protected readonly IFileSystem _fileSystem;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILogger<B> _logger;
    
    /// <inheritdoc />
    public int Order => 99;

    /// <inheritdoc />
    public string Name => "R18Dev";

    public R18DevRemoteProvider(IFileSystem fileSystem,
        IHttpClientFactory httpClientFactory,
        ILogger<B> logger,
        IServerConfigurationManager config
    )
    {
        _config = config;
        _fileSystem = fileSystem;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // public string Name => Constants.PluginName;

    public virtual async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
    {
        _logger.LogDebug("R18 GetMetadata: {Path}", info.Path);
        MetadataResult<Movie> result = new();
        var id = GetJavCode(info.Path);
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogInformation("R18 GetMetadata: JAV Code not found in filename of title: {Name}", info.Name);
            result.HasMetadata = false;
            return result;
        }

        _logger.LogDebug("R18 GetMetadata: Calling Metadata function: {ID}", id);
        var client = _httpClientFactory.CreateClient(Constants.PluginName);
        var response = await client.GetAsync(string.Format(Constants.SearchQuery, id), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("R18 GetMetadata: Error calling Metadata function: {ID}", id);
            result.HasMetadata = false;
            return result;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var search = JsonSerializer.Deserialize<SearchQueryData>(json);

        var movieResponse =
            await client.GetAsync(string.Format(Constants.VideoUrl, search.content_id), cancellationToken);
        var movieJson = await movieResponse.Content.ReadAsStringAsync();
        var movieData = JsonSerializer.Deserialize<MovieData>(movieJson);


        return GetMetadataImpl(movieData);
    }

    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal MetadataResult<Movie> GetMetadataImpl(MovieData movieData)
    {
        return Utils.R18DevMovieDataToMovie(movieData);
    }

    /// <summary>
    ///     Returns the JAV Code from the file path.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetJavCode(string name)
    {
        var match = Regex.Match(name, Constants.JavRe);
        return match.Value;
    }
}