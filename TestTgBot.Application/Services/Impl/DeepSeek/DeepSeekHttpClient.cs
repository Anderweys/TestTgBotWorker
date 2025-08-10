using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TestTgBot.Application.Models.Rest.V1.DeepSeek;
using TestTgBot.Application.Services.Abst.DeepSeek;

namespace TestTgBot.Application.Services.Impl.DeepSeek;

public class DeepSeekHttpClient : IDeepSeekHttpClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _serialize = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    private readonly JsonSerializerOptions _deserialize = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    public DeepSeekHttpClient(HttpClient http)
    {
        _http=http;
    }

    public async Task<string> Post(OpenRouterRequestV1 request, CancellationToken cancellationToken)
    {
        var jsonContent = JsonSerializer.Serialize(request, _serialize);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _http.PostAsync($"chat/completions", httpContent, cancellationToken);
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