using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Plugin.Feliratok.Eu.Configuration;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Feliratok.Eu;

/// <summary>
/// Fetches subtitles from feliratok.eu based on the provided name and language code.
/// </summary>
public class SubtitleFetcher
{
    private readonly ILogger<SubtitleFetcher> _logger;
    private readonly SubtitleParser _subtitleParser;
    private static readonly HttpClient _httpClient = new HttpClient();

    private static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
    {
        { "hu", "Magyar" },
        { "en", "Angol" },
        { "sq", "Albán" },
        { "ar", "Arab" },
        { "bg", "Bolgár" },
        { "pt", "Brazíliai portugál" },
        { "cs", "Cseh" },
        { "da", "Dán" },
        { "fi", "Finn" },
        { "nl", "Holland / Flamand" }, // Dutch / Flemish
        { "fr", "Francia" },
        { "el", "Görög" },
        { "he", "Héber" },
        { "hr", "Horvát" },
        { "ko", "Koreai" },
        { "pl", "Lengyel" },
        { "de", "Német" },
        { "no", "Norvég" },
        { "it", "Olasz" },
        { "ru", "Orosz" },
        { "ro", "Román" },
        { "es", "Spanyol" },
        { "sv", "Svéd" },
        { "sr", "Szerb" },
        { "sl", "Szlovén" },
        { "sk", "Szlovák" },
        { "tr", "Török" }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleFetcher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging.</param>
    /// <param name="subtitleParser">The subtitle fetcher instance to use.</param>
    public SubtitleFetcher(ILogger<SubtitleFetcher> logger, SubtitleParser subtitleParser)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subtitleParser = subtitleParser ?? throw new ArgumentNullException(nameof(subtitleParser));
    }

    /// <summary>
    /// Asynchronously fetches subtitles from feliratok.eu for the specified name and language code.
    /// </summary>
    /// <param name="name">The name of the movie or show to search for subtitles.</param>
    /// <param name="languageCode">The ISO language code (e.g., "hu", "en").</param>
    /// <param name="threeLetterISOLanguageNameCode">The three-letter ISO language name code (e.g., "hun" for Hungarian).</param>
    /// <param name="exactMatch">Whether to require an exact match for the subtitle search.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subtitle content as a string.</returns>
    public async Task<IEnumerable<RemoteSubtitleInfo>> GetSubtitlesAsync(string name, string languageCode, string threeLetterISOLanguageNameCode, bool exactMatch)
    {
        if (!LanguageMap.TryGetValue(languageCode, out var languageName))
        {
            throw new ArgumentException("Invalid language code");
        }

        var url = $"https://feliratok.eu/?search={Uri.EscapeDataString(name)}&nyelv={Uri.EscapeDataString(languageName)}&tab=film";

        var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return await _subtitleParser.ParseResponse(content, name, threeLetterISOLanguageNameCode, exactMatch).ConfigureAwait(false);
        }
        else
        {
            // Handle errors as needed
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
    }

    /// <summary>
    /// Asynchronously downloads the subtitle file from the specified URL.
    /// </summary>
    /// <param name="downloadUrl">The URL to download the subtitle from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subtitle content as a string.</returns>
    public async Task<byte[]> DownloadSubtitleAsync(string downloadUrl)
    {
        _logger.LogInformation("Downloading subtitle from URL: {DownloadUrl}", downloadUrl);
        var response = await _httpClient.GetAsync(downloadUrl).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        else
        {
            // Handle errors as needed
            throw new HttpRequestException($"Download failed with status code {response.StatusCode}");
        }
    }
}
