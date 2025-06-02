using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Feliratok.Eu.Configuration;

/// <summary>
/// Represents the configuration settings for the plugin.
/// Inherits from <see cref="BasePluginConfiguration"/> to provide base configuration functionality.
/// </summary>
/// <summary>
/// The configuration options.
/// </summary>
public enum SourceEncoding
{
    /// <summary>
    /// Latin1.
    /// </summary>
    Latin1,

    /// <summary>
    /// Latin2.
    /// </summary>
    Latin2,

    /// <summary>
    /// UTF-8 option.
    /// </summary>
    UTF8
}

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        SelectedSourceEncoding = SourceEncoding.Latin2;
        ExactMatch = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether some true or false setting is enabled..
    /// </summary>
    public bool ExactMatch { get; set; }

    /// <summary>
    /// Gets or sets an enum option.
    /// </summary>
    public SourceEncoding SelectedSourceEncoding { get; set; }

    /// <summary>
    /// Gets the code page name corresponding to the specified <see cref="SourceEncoding"/>.
    /// </summary>
    /// <param name="encoding">The source encoding.</param>
    /// <returns>The code page name as a string.</returns>
    public static string GetCodePageName(SourceEncoding encoding)
    {
        return encoding switch
        {
            SourceEncoding.Latin1 => "iso-8859-1",
            SourceEncoding.Latin2 => "iso-8859-2",
            SourceEncoding.UTF8 => "utf-8",
            _ => "utf-8"
        };
    }
}