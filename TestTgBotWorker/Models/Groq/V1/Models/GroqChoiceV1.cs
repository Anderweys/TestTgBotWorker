using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.Groq.V1.Models;

public sealed record GroqChoiceV1(
    [property: JsonPropertyName("message")] GroqMessageV1 Message
);