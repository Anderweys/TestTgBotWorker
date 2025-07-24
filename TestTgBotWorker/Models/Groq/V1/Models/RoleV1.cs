using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.Groq.V1.Models;

public record RoleV1
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}