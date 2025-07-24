using Microsoft.Extensions.Options;
using TestTgBotWorker.Configuration.DeepSeek;
using TestTgBotWorker.Models.DeepSeek.V1;
using TestTgBotWorker.Services.Abst.DeepSeek;

namespace TestTgBotWorker.Services.Impl.DeepSeek;

public sealed class DeepSeekService : IDeepSeekService
{
    private readonly IDeepSeekHttpClient _client;
    private readonly DeepSeekOptions _options;

    public DeepSeekService(IDeepSeekHttpClient client, IOptions<DeepSeekOptions> settings)
    {
        _client = client;
        _options = settings.Value;
    }

    public async Task<string> SendSingleMessage(string message, CancellationToken cancellationToken)
    {
        var request = new OpenRouterRequestV1
        {
            Model = _options.Model,
            MaxTokens = _options.Settings.Model.MaxTokens,
            Temperature = _options.Settings.Model.Temperature,
            Messages =
            [
                new()
                {
                    Role = _options.Settings.Role.Role,
                    Content = string.Format(_options.Settings.Role.Content, message)
                }
            ]
        };

        var response = await _client.Post(request, cancellationToken);

        return response;
    }
}