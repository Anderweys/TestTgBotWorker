using Microsoft.Extensions.Options;
using TestTgBot.Application.Configuration.DeepSeek;
using TestTgBot.Application.Models.Rest.V1.DeepSeek;
using TestTgBot.Application.Services.Abst.DeepSeek;

namespace TestTgBot.Application.Services.Impl.DeepSeek;

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