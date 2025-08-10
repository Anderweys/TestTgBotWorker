using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.Groq.Models;

public sealed record GroqMessageV1(
    [property: JsonPropertyName("content")] string Content
);