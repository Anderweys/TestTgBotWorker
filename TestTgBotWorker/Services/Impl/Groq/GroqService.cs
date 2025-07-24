using Microsoft.Extensions.Options;
using TestTgBotWorker.Models.Groq.V1;
using TestTgBotWorker.Models.Groq.V1.Models;
using TestTgBotWorker.Services.Abst.Groq;
using TestTgBotWorker.Configuration.Groq;

namespace TestTgBotWorker.Services.Impl.Groq;

public class GroqService : IGroqService
{
    private readonly IGroqHttpClient _client;
    private readonly GroqOptions _options;

    public GroqService(IGroqHttpClient client, IOptions<GroqOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<string> SendSingleMessage(string message, CancellationToken cancellationToken)
    {
        var request = new GroqRequestV1
        {
            Model = _options.Model,
            Messages =
            [
                new RoleV1
                {
                    Role = _options.Role.Role,
                    Content = string.Format(_options.Role.Content, message)
                }
            ]
        };

        var response = await _client.Post(request, cancellationToken);
        return response;
    }
}