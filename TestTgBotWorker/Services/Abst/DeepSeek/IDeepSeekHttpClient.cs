using TestTgBotWorker.Models.DeepSeek.V1;

namespace TestTgBotWorker.Services.Abst.DeepSeek;

public interface IDeepSeekHttpClient
{
    Task<string> Post(OpenRouterRequestV1 request, CancellationToken cancellationToken);
}