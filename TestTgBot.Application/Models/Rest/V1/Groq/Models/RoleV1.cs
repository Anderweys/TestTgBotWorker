using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.Groq.Models;

public record RoleV1
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}