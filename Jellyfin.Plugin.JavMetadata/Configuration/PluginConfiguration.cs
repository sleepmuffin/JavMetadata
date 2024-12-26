using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JavMetadata.Configuration;

/// <summary>
///     Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginConfiguration" /> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        TrueFalseSetting = true;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether some true or false setting is enabled..
    /// </summary>
    public bool TrueFalseSetting { get; set; }
}