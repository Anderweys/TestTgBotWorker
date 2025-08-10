using System.Text.Json.Serialization;
using TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek;

public record OpenRouterRequestV1
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("messages")]
    public List<ChatMessageV1> Messages { get; set; } = [];

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }
}