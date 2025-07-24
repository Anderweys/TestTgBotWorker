using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.Groq.V1.Models;

public sealed record GroqMessageV1(
    [property: JsonPropertyName("content")] string Content
);