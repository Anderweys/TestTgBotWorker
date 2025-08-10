using System.Xml.Linq;
using Microsoft.Extensions.Options;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Services.Abst.RuleThirtyFour;

namespace TestTgBot.Application.Services.Impl.RuleThirtyFour;

public sealed class RuleThirtyFourHttpClient : IRuleThirtyFourHttpClient
{
    private readonly HttpClient _http;
    private readonly RuleThirtyFourOptions _options;

    public RuleThirtyFourHttpClient(HttpClient http, IOptions<RuleThirtyFourOptions> options)
    {
        _http=http;
        _options=options.Value;
    }

    public async Task<int> GetTotalPostCount(string tags, CancellationToken cancellationToken)
    {
        var countXml = await _http.GetStringAsync(string.Format(_options.BaseUrl, tags, 0), cancellationToken);

        var countDoc = XDocument.Parse(countXml);
        var countAttr = countDoc.Root?.Attribute("count");

        return int.TryParse(countAttr?.Value, out int result)
            ? result
            : 0;
    }

    public async Task<List<string?>> GetPostFileUrls(string tags, int pid, CancellationToken cancellationToken)
    {
        var pageXml = await _http.GetStringAsync(string.Format(_options.BaseUrl, tags, 100) + $"&pid={pid}", cancellationToken);

        return [.. XDocument.Parse(pageXml)
            .Descendants("post")
            .Select(p => p.Attribute("file_url")?.Value)
            .Where(url => !string.IsNullOrEmpty(url) &&
                          (url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                           url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))];
    }

    public async Task<long?> GetRemoteFileSizeAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return response.IsSuccessStatusCode && response.Content.Headers.ContentLength.HasValue
                ? response.Content.Headers.ContentLength.Value
                : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Stream> GetStream(string url, CancellationToken cancellationToken) =>
        await _http.GetStreamAsync(url, cancellationToken);
}