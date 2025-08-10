using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.Groq.Models;

public sealed record GroqChoiceV1(
    [property: JsonPropertyName("message")] GroqMessageV1 Message
);