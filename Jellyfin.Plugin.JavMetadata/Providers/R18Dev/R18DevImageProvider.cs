using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.JavMetadata.Providers.R18Dev;

public class R18DevImageProvider : IRemoteImageProvider, IHasOrder
{
    private static readonly HttpClient HttpClient = new();

    /// <summary>Initializes a new instance of the <see cref="R18DevImageProvider" /> class.</summary>
    public R18DevImageProvider()
    {
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
    }

    /// <inheritdoc />
    public int Order => 99;

    /// <inheritdoc />
    public string Name => "R18Dev";

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
    {
        var id = item.GetProviderId("R18Dev");
        if (string.IsNullOrEmpty(id)) return Array.Empty<RemoteImageInfo>();

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
        // await Utility.CropThumb(httpResponse).ConfigureAwait(false);
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
                // Ignore HttpRequestException and try the next URL
            }
            catch (OperationCanceledException)
            {
                // If the operation is canceled, propagate the cancellation
                throw;
            }
            catch (Exception)
            {
                // Ignore other exceptions and try the next URL
            }
        }

        // If no valid image URL is found, return null
        return null;
    }
}