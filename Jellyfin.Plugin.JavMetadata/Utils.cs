using Jellyfin.Data.Enums;
using Jellyfin.Plugin.JavMetadata.Providers.R18Dev;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using SkiaSharp;

namespace Jellyfin.Plugin.JavMetadata;

public class Utils
{
    public static MetadataResult<Movie> R18DevMovieDataToMovie(MovieData data)
    {
        var item = new Movie();
        var result = new MetadataResult<Movie>
        {
            HasMetadata = true,
            Item = item
        };

        result.Item.Name = $"[{data.dvd_id}] {data.title_en}";
        result.Item.Genres = data.categories
            .FindAll(categoriesData => !categoriesData.name_en_is_machine_translation)
            .ConvertAll(categoriesData => categoriesData.name_en)
            .ToArray();
        result.Item.Overview = data.comment_en;

        var date = new DateTime(1970, 1, 1);
        try
        {
            date = DateTime.ParseExact(data.release_date, "yyyy-MM-dd", null);
        }
        catch
        {
            Console.WriteLine("Error parsing date");
        }

        result.Item.ProductionYear = date.Year;
        result.Item.PremiereDate = date;
        result.Item.ProviderIds = new Dictionary<string, string> { { "R18", data.content_id } };
        result.Item.ExternalId = data.content_id;
        data.actresses.ForEach(actress => { result.AddPerson(CreatePerson(actress)); });

        return result;
    }

    private static PersonInfo CreatePerson(ActressData actress)
    {
        return new PersonInfo
        {
            Name = actress.name_romaji,
            Type = PersonKind.Actor,
            ImageUrl = string.Format(Constants.ImageUrl, actress.image_url),
            ProviderIds = new Dictionary<string, string>
            {
                { "R18", actress.id.ToString() }
            }
        };
    }
    
    /// <summary>Crops a full size dvd cover into just the front cover image. Copied from JellyfinJav</summary>
    /// <param name="httpResponse">The full size dvd cover's http response.</param>
    /// <returns>An empty task when the job is done.</returns>
    public static async Task CropThumb(HttpResponseMessage httpResponse)
    {
        using var imageStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var imageBitmap = SKBitmap.Decode(imageStream);

        SKBitmap subset = new SKBitmap();
        imageBitmap.ExtractSubset(subset, SKRectI.Create(421, 0, 379, 538));

        // I think there will be a memory leak if I use MemoryStore.
        var finalStream = File.Open(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg"), FileMode.OpenOrCreate);
        subset.Encode(finalStream, SKEncodedImageFormat.Jpeg, 100);
        finalStream.Seek(0, 0);

        var newContent = new StreamContent(finalStream);
        newContent.Headers.ContentType = httpResponse.Content.Headers.ContentType;
        httpResponse.Content = newContent;
    }
}