#pragma warning disable IDE1006 // Naming Styles
namespace Jellyfin.Plugin.JavMetadata.Providers.R18Dev;

public class JacketImage
{
    public string large { get; set; }
    public string large2 { get; set; }
}

public class Images
{
    public JacketImage jacket_image { get; set; }
}

public class Label
{
    public string name { get; set; }
}

public class Maker
{
    public string name { get; set; }
}

public class Sample
{
    public string high { get; set; }
}

public class Series
{
    public string name { get; set; }
    public string series_url { get; set; }
}

public class SearchQueryData
{
    public string content_id { get; set; }
    public string title { get; set; }
    public string director { get; set; }
    public Images images { get; set; }
    public Label label { get; set; }
    public Maker maker { get; set; }
    public string release_date { get; set; }
    public short runtime_minutes { get; set; }
    public Sample sample { get; set; }
    public Series series { get; set; }
}