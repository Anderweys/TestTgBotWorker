using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using TestTgBotWorker.Models.Groq.V1;
using TestTgBotWorker.Services.Abst.Groq;

namespace TestTgBotWorker.Services.Impl.Groq;

public sealed class GroqHttpClient : IGroqHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _serialize = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    private readonly JsonSerializerOptions _deserialize = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    public GroqHttpClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory=httpClientFactory;
    }

    public async Task<string> Post(GroqRequestV1 request, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient(nameof(GroqHttpClient));

        try
        {
            var response = await client.PostAsJsonAsync("chat/completions", request, _serialize, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Groq API error {response.StatusCode}: {errorContent}");
            }

            var groqResponse = await response.Content.ReadFromJsonAsync<GroqResponseV1>(_deserialize, cancellationToken);
            return groqResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "Response is empty.";
        }
        catch (JsonException ex)
        {
            return $"Failed to parse Groq API response. Error: {ex}";
        }
    }
}