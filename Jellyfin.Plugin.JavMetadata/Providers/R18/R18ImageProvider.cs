using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Jellyfin.Plugin.JavMetadata.Providers.R18;

public class R18ImageProvider : IRemoteImageProvider, IHasOrder
{
    private static readonly HttpClient HttpClient = new();
    private readonly ILogger<R18ImageProvider> _logger;

    /// <summary>Initializes a new instance of the <see cref="R18ImageProvider" /> class.</summary>
    public R18ImageProvider(ILogger<R18ImageProvider> logger)
    {
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        this._logger = logger;
    }

    /// <inheritdoc />
    public int Order => 99;

    /// <inheritdoc />
    public string Name => "R18";

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
    {
        _logger.LogInformation("[R18] Starting Image Provider for {item}", item.ExternalId);
        var id = item.GetProviderId("R18");
        if (string.IsNullOrEmpty(id)) return Array.Empty<RemoteImageInfo>();
        _logger.LogInformation("[R18] Getting images for {id}", id);

        var primaryImageFormats = new[]
        {
            $"https://awsimgsrc.dmm.com/dig/digital/video/{id}/{id}pl.jpg",
            $"https://pics.dmm.co.jp/mono/movie/adult/{id}/{id}pl.jpg"
        };

        var primaryImage = await this.GetValidImageUrl(primaryImageFormats, cancelToken);

        if (string.IsNullOrEmpty(primaryImage))
            // If no valid image URL is found, return the fallback URL
            primaryImage = $"https://awsimgsrc.dmm.com/dig/digital/video/{id}/{id}pl.jpg";

        if (string.IsNullOrEmpty(primaryImage))
            // If the primary image URL is empty, return an empty collection
            return Array.Empty<RemoteImageInfo>();

        return new[]
        {
            new RemoteImageInfo
            {
                ProviderName = Name,
                Type = ImageType.Primary,
                Url = primaryImage
            }
        };
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
    {
        var httpResponse = await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        await Utils.CropThumb(httpResponse).ConfigureAwait(false);
        return httpResponse;
    }

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        return new[] { ImageType.Primary };
    }
    
    /// <inheritdoc />
    public bool Supports(BaseItem item) => item is Movie;
    
    private async Task<string?> GetValidImageUrl(IEnumerable<string> imageFormats, CancellationToken cancellationToken)
    {
        foreach (var imageUrl in imageFormats)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(imageUrl, cancellationToken))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // If the response is successful, return the URL
                        return imageUrl;
                    }
                }
            }
            catch (HttpRequestException)
            {
                _logger.LogInformation("[R18] Image URL could not be retrieved");
                // Ignore HttpRequestException and try the next URL
            }
            catch (OperationCanceledException)
            {
                // If the operation is canceled, propagate the cancellation
                throw;
            }
            catch (Exception)
            {
                _logger.LogInformation("[R18] Image URL could not be retrieved");
                // Ignore other exceptions and try the next URL
            }
        }

        // If no valid image URL is found, return null
        _logger.LogInformation("[R18] Image URL is null");
        return null;
    }
}