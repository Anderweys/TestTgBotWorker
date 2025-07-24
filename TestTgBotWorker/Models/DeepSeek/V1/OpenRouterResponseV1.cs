using System.Text.Json.Serialization;
using TestTgBotWorker.Models.DeepSeek.V1.Models;

namespace TestTgBotWorker.Models.DeepSeek.V1;

public record OpenRouterResponseV1
{
    [JsonPropertyName("choices")]
    public List<ChoiceV1> Choices { get; set; } = [];

    [JsonPropertyName("usage")]
    public UsageV1? Usage { get; set; }

    [JsonPropertyName("error")]
    public ApiErrorV1? Error { get; set; }
}