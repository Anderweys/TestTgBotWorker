using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

public record UsageV1
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}