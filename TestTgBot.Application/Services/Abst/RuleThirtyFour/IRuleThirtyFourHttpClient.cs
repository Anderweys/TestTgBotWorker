namespace TestTgBot.Application.Services.Abst.RuleThirtyFour;

public interface IRuleThirtyFourHttpClient
{
    Task<int> GetTotalPostCount(string tags, CancellationToken cancellationToken);
    Task<List<string?>> GetPostFileUrls(string tags, int pid, CancellationToken cancellationToken);
    Task<long?> GetRemoteFileSizeAsync(string url, CancellationToken cancellationToken);
    Task<Stream> GetStream(string url, CancellationToken cancellationToken);
}