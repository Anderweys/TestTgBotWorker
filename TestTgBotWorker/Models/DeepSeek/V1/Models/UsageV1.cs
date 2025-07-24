using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.DeepSeek.V1.Models;

public record UsageV1
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}