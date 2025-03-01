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
        result.Item.ProviderIds = new Dictionary<string, string> { { "MetadataServe", data.contentId } };
        result.Item.ExternalId = data.contentId;
        data.actresses.ForEach(actress => { result.AddPerson(CreatePerson(actress)); });

        return result;
    }

    private static PersonInfo CreatePerson(ActressData actress)
    {
        return new PersonInfo
        {
            Name = actress.nameRomaji ?? actress.nameKanji,
            Type = PersonKind.Actor,
            ImageUrl = string.Format(Constants.ImageUrl, actress.imageUrl),
            ProviderIds = new Dictionary<string, string>
            {
                { "MetadataServe", actress.id.ToString() }
            }
        };
    }
}