using Jellyfin.Data.Enums;
using Jellyfin.Plugin.JavMetadata.Providers.R18Dev;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;

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

        result.Item.Name = data.title_en;
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
        result.Item.ProviderIds = new Dictionary<string, string> { { "R18Dev", data.dvd_id } };
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
                { "R18Dev", actress.id.ToString() }
            }
        };
    }
}