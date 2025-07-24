using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.DeepSeek.V1.Models;

public record ChatMessageV1
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}