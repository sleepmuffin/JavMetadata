using System.Text.Json;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.JavMetadata.Models;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JavMetadata.Providers.MetadataServe;

public class MetadataServeProvider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
{
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILogger<MetadataServeProvider> _logger;
    
    /// <inheritdoc />
    public int Order => 99;

    /// <inheritdoc />
    public string Name => "MetadataServe";

    /// <summary>Initializes a new instance of the <see cref="R18Provider"/> class.</summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory" />.</param>
    /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
    public MetadataServeProvider(IHttpClientFactory httpClientFactory, ILogger<MetadataServeProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public virtual async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MetadataServe GetMetadata: {Path}", info.Path);
        MetadataResult<Movie> result = new();
        var externalId = info.GetProviderId("MetadataServe");
        var id = externalId ?? GetJavCode(info.Path);
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogInformation("MetadataServe GetMetadata: JAV Code not found in filename of title: {Name}", info.Name);
            result.HasMetadata = false;
            return result;
        }

        _logger.LogInformation("MetadataServe GetMetadata: Calling Metadata function: {ID}", id);
        var client = _httpClientFactory.CreateClient(Constants.PluginName);
        var response = await client.GetAsync(string.Format(Constants.MetadataServeSearchQuery, id), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("MetadataServe GetMetadata: Error calling Metadata function: {ID}", id);
            _logger.LogInformation("MetadataServe GetMetadata Response: {response}", response);
            result.HasMetadata = false;
            return result;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Deserializing first call: {json}", json);
        var movieData = JsonSerializer.Deserialize<MetadataServeMovieData>(json);
        return GetMetadataImpl(movieData);
    }

    /// <inheritdoc />
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(Constants.PluginName);
        return await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
    }

    internal MetadataResult<Movie> GetMetadataImpl(MetadataServeMovieData movieData)
    {
        return Models.Utils.MetadataServeDataToMovie(movieData);
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