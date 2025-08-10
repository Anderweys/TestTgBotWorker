using System.Text.Json.Serialization;
using TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek;

public record OpenRouterResponseV1
{
    [JsonPropertyName("choices")]
    public List<ChoiceV1> Choices { get; set; } = [];

    [JsonPropertyName("usage")]
    public UsageV1? Usage { get; set; }

    [JsonPropertyName("error")]
    public ApiErrorV1? Error { get; set; }
}