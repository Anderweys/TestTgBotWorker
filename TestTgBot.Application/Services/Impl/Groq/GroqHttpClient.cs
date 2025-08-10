using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TestTgBot.Application.Models.Rest.V1.Groq;
using TestTgBot.Application.Services.Abst.Groq;

namespace TestTgBot.Application.Services.Impl.Groq;

public sealed class GroqHttpClient : IGroqHttpClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _serialize = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    private readonly JsonSerializerOptions _deserialize = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    public GroqHttpClient(HttpClient http)
    {
        _http=http;
    }

    public async Task<string> Post(GroqRequestV1 request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("chat/completions", request, _serialize, cancellationToken);

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