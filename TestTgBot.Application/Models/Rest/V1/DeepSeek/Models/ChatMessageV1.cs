using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

public record ChatMessageV1
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}