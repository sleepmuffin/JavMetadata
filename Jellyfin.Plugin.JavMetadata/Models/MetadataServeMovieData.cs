namespace Jellyfin.Plugin.JavMetadata.Models;

public class MetadataServeMovieData
{
    public List<ActressData> actresses { get; set; }
    public List<CategoriesData> categories { get; set; }
    public string? commentEn { get; set; }
    public string? contentId { get; set; }
    public string? dvdId { get; set; }
    public string? jacketFullUrl { get; set; }
    public string? releaseDate { get; set; }
    public string? titleEn { get; set; }
    public string? titleJa { get; set; }
}

public class CategoriesData
{
    public long id { get; set; }
    public string? name { get; set; }
}

public class ActressData
{
    public long id { get; set; }
    public string? imageUrl { get; set; }
    public string? nameKanji { get; set; }
    public string? nameRomaji { get; set; }
}