namespace Jellyfin.Plugin.JavMetadata;

public class Constants
{
    // public const string VideoUrl = "https://r18.dev/videos/vod/movies/detail/-/id=h_068mxgs1364/";

    public const string SearchQuery = "https://r18.dev/videos/vod/movies/detail/-/dvd_id={0}/json";

    // For a complete search using the content_id of the response from the search query 
    public const string VideoUrl = "https://r18.dev/videos/vod/movies/detail/-/combined={0}/json";
    public const string PluginGuid = "6a5c304c-6ef6-4150-a3fe-e8a9b2f6492b";
    public const string ImageUrl = "https://pics.dmm.co.jp/mono/actjpgs/{0}";
    public const string PluginName = "JAVMetadata";
    public const string JavRe = "([A-Z-]+[\\d]+)";
}