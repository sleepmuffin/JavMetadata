namespace Jellyfin.Plugin.JavMetadata;

public class Constants
{
    // public const string VideoUrl = "https://r18.dev/videos/vod/movies/detail/-/id=h_068mxgs1364/";

    public const string SearchQuery = "https://r18.dev/videos/vod/movies/detail/-/dvd_id={0}/json";

    // For a complete search using the content_id of the response from the search query 
    public const string VideoUrl = "https://r18.dev/videos/vod/movies/detail/-/combined={0}/json";
    public const string PluginGuid = "3dd15d34-f65b-441d-94a7-788081fc96ea";
    public const string ImageUrl = "https://pics.dmm.co.jp/mono/actjpgs/{0}";
    public const string PluginName = "JAVMetadata";
    public const string JavRe = "([A-Z-]+[\\d]+)";
}