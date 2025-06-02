using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Feliratok.Eu;

/// <summary>
/// Parses HTML content to extract subtitle information from a specific table row.
/// </summary>
public class SubtitleParser
{
    private readonly ILogger<SubtitleParser> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleParser"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    public SubtitleParser(ILogger<SubtitleParser> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses the provided HTML content and extracts subtitle information from all table rows with id 'vilagit'.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <param name="searchTitle">The title to search for in the subtitles.</param>
    /// <param name="threeLetterISOLanguageNameCode">The three-letter ISO language name code (e.g., "hun" for Hungarian).</param>
    /// <param name="exactMatch">Whether to require an exact match for the title.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<IEnumerable<RemoteSubtitleInfo>> ParseResponse(string htmlContent, string searchTitle, string threeLetterISOLanguageNameCode, bool exactMatch)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Select all <tr> with id="vilagit"
        var rows = htmlDoc.DocumentNode.SelectNodes("//tr[@id='vilagit']");
        var titleInfoList = new List<RemoteSubtitleInfo>();

        if (rows != null)
        {
            foreach (var row in rows)
            {
                // Extract the download link
                var downloadLinkNode = row.SelectSingleNode(".//td[6]/a");
                var downloadUrl = downloadLinkNode?.GetAttributeValue("href", string.Empty);

                // Extract the language name
                var languageNode = row.SelectSingleNode(".//td[@class='lang']/small");
                var language = languageNode?.InnerText.Trim();

                // Extract the title (inside <div class="magyar">)
                var titleNode = row.SelectSingleNode(".//td[3]/div[@class='magyar']");
                var title = titleNode?.InnerText.Trim();

                // Extract the description
                var descriptionNode = row.SelectSingleNode(".//td[3]/div[@class='eredeti']");
                var description = descriptionNode?.InnerText.Trim();

                var detailedInfoNode = row.SelectSingleNode(".//td[3]");
                var detailedInfo = detailedInfoNode?.GetAttributeValue("onclick", string.Empty);
                string detailedInfoId = string.Empty;
                if (!string.IsNullOrEmpty(detailedInfo))
                {
                    var start = detailedInfo.IndexOf("('", StringComparison.Ordinal) + 2;
                    var end = detailedInfo.IndexOf("')", start, StringComparison.Ordinal);
                    if (start > 1 && end > start)
                    {
                        detailedInfoId = detailedInfo.Substring(start, end - start);
                    }
                }

                string downloadId = string.Empty;
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    var start = downloadUrl.LastIndexOf('=') + 1;
                    downloadId = downloadUrl.Substring(start);
                }

                // Extract the date
                var dateNode = row.SelectSingleNode(".//td[5]");
                var date = dateNode?.InnerText.Trim();

                _logger.LogInformation("Title: {Title}", title);
                _logger.LogInformation("Description: {Description}", description);
                _logger.LogInformation("Language: {LanguageCode}", threeLetterISOLanguageNameCode);
                _logger.LogInformation("Download link: {DownloadId}", downloadId);
                _logger.LogInformation("Date: {Date}", date);
                _logger.LogInformation("DetailedInfoId: {DetailedInfoId}", detailedInfoId);

                if (!exactMatch || (!string.IsNullOrEmpty(title) && title.Replace("(SubRip)", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().Equals(searchTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    titleInfoList.Add(new RemoteSubtitleInfo
                    {
                        Id = $"srt-{threeLetterISOLanguageNameCode}-{downloadId}",
                        Name = title,
                        ProviderName = "FeliratokEu",
                        ThreeLetterISOLanguageName = threeLetterISOLanguageNameCode,
                        DateCreated = DateTime.TryParse(date, out var parsedDate) ? parsedDate : DateTime.MinValue,
                        Format = "srt", // Assuming the format is SRT, adjust as necessary
                        Comment = description,
                    });
                }
            }
        }
        else
        {
            _logger.LogWarning("No rows found.");
        }

        return Task.FromResult<IEnumerable<RemoteSubtitleInfo>>(titleInfoList);
    }
}
