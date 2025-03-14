using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;

namespace Jellyfin.Plugin.JavMetadata.Models;

public class Utils
{
    public static MetadataResult<Movie> MetadataServeDataToMovie(MetadataServeMovieData data)
    {
        var item = new Movie();
        var result = new MetadataResult<Movie>
        {
            HasMetadata = true,
            Item = item
        };

        result.Item.Name = $"[{data.dvdId}] {data.titleEn ?? data.titleJa}";
        result.Item.Genres = data.categories
            .ConvertAll(categoriesData => categoriesData.name)
            .ToArray();
        result.Item.Overview = data.commentEn;

        var date = new DateTime(1970, 1, 1);
        try
        {
            date = DateTime.ParseExact(data.releaseDate ?? "2000-01-01", "yyyy-MM-dd", null);
        }
        catch
        {
            Console.WriteLine("Error parsing date");
        }

        result.Item.ProductionYear = date.Year;
        result.Item.PremiereDate = date;
        result.Item.ProviderIds = new Dictionary<string, string> { { "MetadataServe", data.dvdId } };
        result.Item.ExternalId = data.dvdId;
        result.Item.OriginalTitle = data.titleJa;
        data.actresses.ForEach(actress => { result.AddPerson(CreatePerson(actress, PersonKind.Actor)); });

        // todo add tags for series
        // result.Item.Tags = data.tags
        // result.AddPerson();
        // Type = PersonKind.Director,
        return result;
    }

    private static PersonInfo CreatePerson(ActressData actress, PersonKind personKind)
    {
        string? actressName = null;
        if (actress.imageUrl != null)
        {
            Uri uri = new Uri(actress.imageUrl);
            actressName = uri.Segments.LastOrDefault();
        }
        
        if (actressName != null)
        {
            int fileExtPos = actressName.LastIndexOf(".");
            if (fileExtPos >= 0)
                actressName =
                    System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        actressName
                            .Substring(0, fileExtPos)
                            .Replace("_", " ")
                    );
        }
        return new PersonInfo
        {
            Name = actress.nameRomaji ?? actressName ?? actress.nameKanji,
            Type = personKind,
            ImageUrl = actress.imageUrl,
            ProviderIds = new Dictionary<string, string>
            {
                { "MetadataServe", actress.id.ToString() }
            }
        };
    }
}