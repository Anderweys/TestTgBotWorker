using System.Text.Json.Serialization;
using TestTgBotWorker.Models.Groq.V1.Models;

namespace TestTgBotWorker.Models.Groq.V1;

public record GroqRequestV1
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("messages")]
    public RoleV1[] Messages { get; set; } = [];
}