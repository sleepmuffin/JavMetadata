namespace Jellyfin.Plugin.JavMetadata.Providers.R18Dev;

public class MovieData
{
    public List<object> actors { get; set; }
    public List<ActressData> actresses { get; set; }
    public List<object> authors { get; set; }
    public List<CategoriesData> categories { get; set; }
    public string? comment_en { get; set; }
    public string? content_id { get; set; }
    public List<object> directors { get; set; }
    public string? dvd_id { get; set; }
    public List<GalleryData> gallery { get; set; }
    public List<object> histrions { get; set; }
    public string? jacket_full_url { get; set; }
    public string? jacket_thumb_url { get; set; }
    public int? label_id { get; set; }
    public string? label_name_en { get; set; }
    public string? label_name_ja { get; set; }
    public int? maker_id { get; set; }
    public string? maker_name_en { get; set; }
    public string? maker_name_ja { get; set; }
    public string? release_date { get; set; }
    public short? runtime_mins { get; set; }
    public string? sample_url { get; set; }
    public int? series_id { get; set; }
    public string? series_name_en { get; set; }
    public bool series_name_en_is_machine_translation { get; set; }
    public string? series_name_ja { get; set; }
    public string? service_code { get; set; }
    public int? site_id { get; set; }
    public string? title_en { get; set; }
    public bool title_en_is_machine_translation { get; set; }
    public string? title_ja { get; set; }
}

public class ActressData
{
    public long id { get; set; }
    public string? image_url { get; set; }
    public string? name_kana { get; set; }
    public string? name_kanji { get; set; }
    public string? name_romaji { get; set; }
}

public class CategoriesData
{
    public long id { get; set; }
    public string? name_en { get; set; }
    public bool name_en_is_machine_translation { get; set; }
    public string? name_ja { get; set; }
}

public class GalleryData
{
    public string? image_full { get; set; }
    public string? image_thumb { get; set; }
}