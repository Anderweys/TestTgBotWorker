using System.Text.Json.Serialization;
using TestTgBotWorker.Models.DeepSeek.V1.Models;

namespace TestTgBotWorker.Models.DeepSeek.V1;

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