using System.Text.Json.Serialization;
using TestTgBot.Application.Models.Rest.V1.Groq.Models;

namespace TestTgBot.Application.Models.Rest.V1.Groq;

public record GroqRequestV1
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.8;

    [JsonPropertyName("messages")]
    public RoleV1[] Messages { get; set; } = [];
}