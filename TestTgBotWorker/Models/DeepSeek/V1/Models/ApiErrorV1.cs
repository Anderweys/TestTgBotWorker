using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.DeepSeek.V1;

public record ApiErrorV1
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}