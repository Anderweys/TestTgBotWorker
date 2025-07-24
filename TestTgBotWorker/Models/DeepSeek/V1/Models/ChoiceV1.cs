using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.DeepSeek.V1.Models;

public record ChoiceV1
{
    [JsonPropertyName("message")]
    public ChatMessageV1 Message { get; set; } = new();
}