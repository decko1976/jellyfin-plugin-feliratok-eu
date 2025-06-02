using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Subtitles;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Feliratok.Eu;

/// <summary>
/// Provides subtitle downloading functionality for the Feliratok.eu plugin.
/// </summary>
public class FeliratokEuDownloader : ISubtitleProvider
{
    private readonly ILogger<FeliratokEuDownloader> _logger;
    private readonly SubtitleFetcher _subtitleFetcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeliratokEuDownloader"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{FeliratokEuDownloader}"/> interface.</param>
    /// <param name="subtitleFetcher">The <see cref="SubtitleFetcher"/> used for fetching subtitles.</param>
    public FeliratokEuDownloader(
        ILogger<FeliratokEuDownloader> logger,
        SubtitleFetcher subtitleFetcher)
    {
        Instance = this;
        _logger = logger;
        _subtitleFetcher = subtitleFetcher;
    }

    /// <summary>
    /// Gets the downloader instance.
    /// </summary>
    public static FeliratokEuDownloader? Instance { get; private set; }

    /// <inheritdoc />
    public string Name => "FeliratokEu";

    /// <inheritdoc />
    public IEnumerable<VideoContentType> SupportedMediaTypes
        => new[] { VideoContentType.Movie };

    /// <inheritdoc />
    public Task<SubtitleResponse> GetSubtitles(string id, CancellationToken cancellationToken)
        => GetSubtitlesInternal(id, cancellationToken);

    /// <inheritdoc />
    public Task<IEnumerable<RemoteSubtitleInfo>> Search(SubtitleSearchRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Search called with request: {@Request}", request);
        return _subtitleFetcher.GetSubtitlesAsync(request.Name, request.TwoLetterISOLanguageName.Substring(0, 2), request.Language);
    }

    private async Task<SubtitleResponse> GetSubtitlesInternal(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSubtitles called with id: {Id}", id);
        string[] idParts = id.Split('-');
        var url = $"https://feliratok.eu/index.php?action=letolt&felirat={idParts[2]}";

        byte[] subtitleContent = await _subtitleFetcher.DownloadSubtitleAsync(url).ConfigureAwait(false);
        SubtitleResponse response = new()
        {
            Format = "srt",
            // Stream = new MemoryStream(Encoding.Latin1.GetBytes(subtitleContent)),
            // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Stream = new MemoryStream(Encoding.Convert(Encoding.GetEncoding("iso-8859-2"), Encoding.UTF8, subtitleContent)),
            Language = idParts[1],
        };
        return response;
    }
}