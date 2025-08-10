using TestTgBot.Application.Models.Rest.V1.DeepSeek;

namespace TestTgBot.Application.Services.Abst.DeepSeek;

public interface IDeepSeekHttpClient
{
    Task<string> Post(OpenRouterRequestV1 request, CancellationToken cancellationToken);
}