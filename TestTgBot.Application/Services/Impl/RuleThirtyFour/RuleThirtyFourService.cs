using TestTgBot.Application.Models.Domain.RuleThirtyFour;
using TestTgBot.Application.Models.Enum;
using TestTgBot.Application.Services.Abst.RuleThirtyFour;

namespace TestTgBot.Application.Services.Impl.RuleThirtyFour;

public class RuleThirtyFourService : IRuleThirtyFourService
{
    private readonly Dictionary<ChatChannelType, long> _maxMediaSize = new()
    {
        [ChatChannelType.Photo] = 10 * 1024 * 1024, // 10 MB.
        [ChatChannelType.Gif] = 20 * 1024 * 1024,   // 20 MB.
        [ChatChannelType.Video] = 50 * 1024 * 1024  // 50 MB.
    };
    private readonly IRuleThirtyFourHttpClient _http;

    public RuleThirtyFourService(IRuleThirtyFourHttpClient http)
    {
        _http=http;
    }

    public async Task<RuleThirtyFourMediaResult?> GetRandomMediaUrl(string tagString, CancellationToken cancellationToken)
    {
        var tags = ParseTags(tagString);

        var totalCount = await _http.GetTotalPostCount(tags, cancellationToken);
        if (totalCount == 0)
        {
            return null;
        }

        var rand = new Random();
        var pid = rand.Next(0, Math.Max(1, totalCount / 100));

        var fileUrls = await _http.GetPostFileUrls(tags, pid, cancellationToken);
        if (fileUrls.Count == 0)
        {
            return null;
        }

        var selectedUrl = fileUrls[rand.Next(fileUrls.Count)];
        var type = GetMediaType(selectedUrl!);

        if (selectedUrl is not null)
        {
            var size = await _http.GetRemoteFileSizeAsync(selectedUrl, cancellationToken);
            if (size is null || size > _maxMediaSize[type])
            {
                return null;
            }
        }

        return new RuleThirtyFourMediaResult
        {
            Url = selectedUrl!,
            Type = type
        };
    }

    private static string ParseTags(string raw)
    {
        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var tags = parts.Select(t => t.Replace(' ', '_'));
        return string.Join("+", tags);
    }

    private static ChatChannelType GetMediaType(string url) =>
        Path.GetExtension(url)?.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" or ".png" => ChatChannelType.Photo,
            ".gif" => ChatChannelType.Gif,
            ".webm" or ".mp4" => ChatChannelType.Video,
            _ => ChatChannelType.Unknown
        };

    public async Task<Stream> GetMediaStream(RuleThirtyFourMediaResult mediaResult, CancellationToken cancellationToken) =>
        await _http.GetStream(mediaResult.Url, cancellationToken);
}