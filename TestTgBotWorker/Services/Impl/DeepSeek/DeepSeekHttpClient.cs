using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using TestTgBotWorker.Models.DeepSeek.V1;
using TestTgBotWorker.Services.Abst.DeepSeek;
using TestTgBotWorker.Configuration.DeepSeek;

namespace TestTgBotWorker.Services.Impl.DeepSeek;

public class DeepSeekHttpClient : IDeepSeekHttpClient
{
    private readonly DeepSeekOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _serialize = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    private readonly JsonSerializerOptions _deserialize = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    public DeepSeekHttpClient(IHttpClientFactory httpClientFactory, IOptions<DeepSeekOptions> options)
    {
        _httpClientFactory=httpClientFactory;
        _options=options.Value;
    }

    public async Task<string> Post(OpenRouterRequestV1 request, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient(nameof(DeepSeekHttpClient));

        var jsonContent = JsonSerializer.Serialize(request, _serialize);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync($"chat/completions", httpContent, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return $"Error status code: {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseModel = JsonSerializer.Deserialize<OpenRouterResponseV1>(responseContent, _deserialize);
            if (responseModel?.Error != null)
            {
                return $"Error response message: {responseModel.Error.Message}";
            }

            if (responseModel?.Choices?.Count > 0)
            {
                return responseModel.Choices[0].Message.Content.Trim();
            }

            return "Response is empty";
        }

        catch (Exception ex)
        {
            return $"Error exception: {ex}";
        }
    }
}